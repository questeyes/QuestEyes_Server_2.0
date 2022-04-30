using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace QuestEyes_Server.Functions
{
    internal class DeviceConnectivity : App
    {
        /** 
        * PORTS:
        *  7579 device discovery port
        *  7580 command/ota/stream socket
       **/

        //Networking variables
        public static UdpClient? DiscoverPort { get; set; }
        public static ClientWebSocket? CommunicationSocket { get; set; }

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
        public static System.Timers.Timer? HeartbeatTimer { get; set; }
        public static System.Timers.Timer? ConnectionTimeoutTimer { get; set; }

        public static async Task Search()
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

                        //SupportFunctions.outConsole("Searching for device...");
                        
                        
                        //mainWindowViewModel.ChangeBatteryStatusDisplay(null);
                        //mainWindowViewModel.ChangeFirmwareVersionDisplay(null);
                        Debug.Print("Changed status to searching");
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
                            //SupportFunctions.outConsole("Detected " + hostname);
                            //SupportFunctions.outConsole("Attempting connection to " + hostname);
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
            //mainWindowViewModel.ChangeStatus("connecting", null);
            Debug.Print("Changed status to connecting");

            ConnectionTimeoutTimer = new System.Timers.Timer(10000);
            ConnectionTimeoutTimer.Elapsed += OnConnectionTimeout;
            ConnectionTimeoutTimer.AutoReset = true;
            ConnectionTimeoutTimer.Enabled = true;

            try
            {
                await CommunicationSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            }
            catch
            {
                //ignore
            }

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
            //SupportFunctions.outConsole("ERROR: Failed to establish connection to device.");
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
                //SupportFunctions.outConsole("Successful connection confirmed");
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
                //mainWindowViewModel.ChangeStatus("connected", split[1]);
                Debug.Print("Changed status to connected");
                DeviceName = split[1];
                return;
            }
            if (messageText.Contains("FIRMWARE_VER"))
            {
                string[] split = messageText.Split(' ');
                DeviceFirmware = split[1];
                //mainWindowViewModel.ChangeFirmwareVersionDisplay(DeviceFirmware);
                Debug.Print("Updated device firmware version display");
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
                //SupportFunctions.outConsole("ERROR: Device reported excessive frame failure, disconnecting...");
                HeartbeatTimer.Stop();
                HeartbeatTimer.Close();
                CloseCommunicationSocket(CommunicationSocket);
                return;
            }
            if (messageText.Contains("OTA_MODE_ACTIVE"))
            {
                //(SupportFunctions.outConsole("Device has entered OTA mode.");
                //mainWindowViewModel.ChangeStatus("ota", null);
                Debug.Print("Updated display to match OTA mode");
                DeviceMode = "OTA";
            }
            else
            {
                //SupportFunctions.outConsole("Invalid command received from device: " + messageText);
            }
        }

        private static void BinaryReceive(MemoryStream ms)
        {
            //(int right_X, int right_Y, int left_X, int left_Y) = EyeTrackingFramework.detectEyes(ms.ToArray());
            //
        }

        private static void OnHeartbeatFailure(object sender, ElapsedEventArgs e)
        {
            //heartbeat was failed to be received within 10 seconds...
            //SupportFunctions.outConsole("ERROR: Device timed out, Disconnecting...");
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
