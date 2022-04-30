using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.IO;

namespace QuestEyes_Server.Models
{
    internal class DeviceConnectivity : App
    {
        /** 
        * PORTS:
        *  7579 device discovery port
        *  7580 command/ota/stream socket
       **/

        //Networking variables
        public static UdpClient DiscoverPort { get; set; } = default!;
        public static ClientWebSocket CommunicationSocket { get; set; } = default!;

        public static bool Connected { get; set; }
        public static bool AttemptingConnection { get; set; }
        public static string? Url { get; set; }
        public static string? PacketContents { get; set; }
        public static string[] PacketContentsFormatted { get; set; } = Array.Empty<string>();

        //Device info variables
        public static string? DeviceIP { get; set; }
        public static string? DeviceName { get; set; }
        public static string? DeviceFirmware { get; set; }
        public static string DeviceMode { get; set; } = "NORMAL";

        //Timers
        public static System.Timers.Timer HeartbeatTimer { get; set; } = default!;
        public static System.Timers.Timer ConnectionTimeoutTimer { get; set; } = default!;

        public static async Task SetupAndSearch()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    while (!Connected && !AttemptingConnection)
                    {
                        if (HeartbeatTimer != null)
                        {
                            HeartbeatTimer.Stop();
                            HeartbeatTimer.Close();
                        }
                        if (ConnectionTimeoutTimer != null)
                        {
                            ConnectionTimeoutTimer.Stop();
                            ConnectionTimeoutTimer.Close();
                        }

                        Functions.UIFunctions.PrintToConsole("Searching for device...");
                        Functions.UIFunctions.SetStatus("searching", null);
                        Functions.UIFunctions.SetBatteryText("Not connected");
                        Functions.UIFunctions.SetFirmwareText("Not connected");
                        Url = null;
                        PacketContents = null;
                        PacketContentsFormatted = Array.Empty<string>();
                        DiscoverPort = new UdpClient(7579);
                        var receivedResults = await DiscoverPort.ReceiveAsync();
                        PacketContents = Encoding.ASCII.GetString(receivedResults.Buffer);
                        PacketContentsFormatted = PacketContents.Split(new char[] { ':' });
                        if (PacketContentsFormatted[0] == ("QUESTEYE_REQ_CONN"))
                        {
                            DiscoverPort.Close();
                            AttemptingConnection = true;
                            string hostname = PacketContentsFormatted[1];
                            DeviceIP = PacketContentsFormatted[2];
                            Functions.UIFunctions.PrintToConsole("Detected " + hostname + ".");
                            Functions.UIFunctions.PrintToConsole("Attempting connection to " + hostname + ".");
                            Url = "ws://" + DeviceIP + ":7580";
                            CommunicationSocket = new ClientWebSocket();
                            await ConnectAsync(Url);
                        }
                    }
                }
            });
        }

        public static async Task ConnectAsync(string url)
        {

            Functions.UIFunctions.SetStatus("connecting", null);
            ConnectionTimeoutTimer = new System.Timers.Timer(10000);
            ConnectionTimeoutTimer.Elapsed += OnConnectionTimeout;
            ConnectionTimeoutTimer.AutoReset = true;
            ConnectionTimeoutTimer.Enabled = true;

            try
            {
                await CommunicationSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            }
            catch { /* safe to ignore here */ }

            do
            {
                try
                {
                    await Receive(CommunicationSocket);
                }
                catch
                {
                    CloseCommunicationSocket(CommunicationSocket);
                }
            } while (Connected);
        }

        private static void OnConnectionTimeout(object sender, ElapsedEventArgs e)
        {
            //connection failed within 10 seconds...
            ConnectionTimeoutTimer.Stop();
            ConnectionTimeoutTimer.Close();
            Functions.UIFunctions.PrintToConsole("ERROR: Failed to establish connection to device.");
            CommunicationSocket.Abort();
            CommunicationSocket.Dispose();
            AttemptingConnection = false;
        }

        static async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using var ms = new MemoryStream();
                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    //websocket is closed
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Websocket Closed", CancellationToken.None);
                    socket.Dispose();
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    //message is a text message
                    await TextReceiveAsync(ms);
                    return;
                }

                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    //message is binary
                    BinaryReceive(ms);
                    return;
                }
            } while (true);
        }

        private static async Task TextReceiveAsync(MemoryStream ms)
        {
            //IF NAME, TREAT AS DEVICE NAME
            //IF FIRMWARE_VER, TREAT AS FIRMWARE VERSION
            //IF BATTERY, TREAT AS BATTERY LEVEL
            //IF EXCESSIVE_FRAME_FAILURE, TREAT AS DEVICE ERROR, DISCONNECT AND EXPECT REBOOT

            string messageText;

            ms.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(ms, Encoding.UTF8))
                messageText = await reader.ReadToEndAsync();

            if (messageText.Contains("NAME"))
            {
                ConnectionTimeoutTimer.Stop();
                ConnectionTimeoutTimer.Close();
                Connected = true;
                AttemptingConnection = false;
                HeartbeatTimer = new System.Timers.Timer(10000);
                HeartbeatTimer.Elapsed += OnHeartbeatFailure;
                HeartbeatTimer.AutoReset = true;
                HeartbeatTimer.Enabled = true;
                /*Main.ReconnectButton.Invoke((MethodInvoker)delegate
                {
                    Main.ReconnectButton.Enabled = true;
                    Main.UpdateButton.Enabled = true;
                });*/
                string[] split = messageText.Split(' ');
                DeviceName = split[1];
                Functions.UIFunctions.SetStatus("connected", DeviceName);
                Functions.UIFunctions.PrintToConsole("Successful connection confirmed.");
                return;
            }
            if (messageText.Contains("FIRMWARE_VER"))
            {
                string[] split = messageText.Split(' ');
                DeviceFirmware = split[1];
                Functions.UIFunctions.SetFirmwareText(DeviceFirmware);
                Functions.UIFunctions.PrintToConsole("Device reported firmware version " + DeviceFirmware + ".");
                return;
            }
            if (messageText.Contains("BATTERY"))
            {
                //TODO: ADD BATTERY PARSING
                return;
            }
            if (messageText.Contains("HEARTBEAT"))
            {
                HeartbeatTimer.Interval = 10000;
                return;
            }
            if (messageText.Contains("EXCESSIVE_FRAME_FAILURE"))
            {
                Functions.UIFunctions.PrintToConsole("ERROR: Device reported excessive frame failure, disconnecting...");
                HeartbeatTimer.Stop();
                HeartbeatTimer.Close();
                CloseCommunicationSocket(CommunicationSocket);
                return;
            }
            if (messageText.Contains("OTA_MODE_ACTIVE"))
            {
                Functions.UIFunctions.PrintToConsole("Device has entered OTA mode.");
                Functions.UIFunctions.SetStatus("ota", null);
                DeviceMode = "OTA";
            }
            else
            {
                Functions.UIFunctions.PrintToConsole("Invalid command received from device: " + messageText + ".");
            }
        }

        private static void BinaryReceive(MemoryStream ms)
        {
            //(int right_X, int right_Y, int left_X, int left_Y) = EyeTrackingFramework.detectEyes(ms.ToArray());
        }

        private static void OnHeartbeatFailure(object sender, ElapsedEventArgs e)
        {
            //heartbeat was failed to be received within 10 seconds...
            Functions.UIFunctions.PrintToConsole("ERROR: Device timed out, Disconnecting...");
            HeartbeatTimer.Stop();
            HeartbeatTimer.Close();
            CloseCommunicationSocket(CommunicationSocket);
        }

        public static void CloseCommunicationSocket(WebSocket socket)
        {
            socket.Dispose();
            Connected = false;
            AttemptingConnection = false;

            /*if (DiagnosticsPanel.DiagnosticsOpen)
            {
                DiagnosticsPanel.DecodeError.Invoke((MethodInvoker)delegate
                {
                    DiagnosticsPanel.DecodeError.Visible = true;
                });
            }*/
        }
    }
}
