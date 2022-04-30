using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.IO;
using Avalonia.Media;

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

        //Colours for status label
        public static readonly SolidColorBrush red = new();
        public static readonly SolidColorBrush orange = new();
        public static readonly SolidColorBrush green = new();
        public static readonly SolidColorBrush purple = new();

        public static async Task SetupAndSearch()
        {
            //setup the label colours
            red.Color = Color.FromRgb(255, 0, 0);
            orange.Color = Color.FromRgb(255, 165, 0);
            green.Color = Color.FromRgb(0, 255, 0);
            purple.Color = Color.FromRgb(138, 43, 226);

            //start search
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

                        Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nSearching for device...");
                        Views.MainWindow.StatusLabelText.OnNext("Searching...");
                        Views.MainWindow.StatusLabelColour.OnNext(red);
                        Views.MainWindow.BatteryLabelText.OnNext("Not connected");
                        Views.MainWindow.FirmwareLabelText.OnNext("Not connected");
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
                            Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nDetected " + hostname);
                            Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nAttempting connection to " + hostname);
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
            
            Views.MainWindow.StatusLabelText.OnNext("Connecting...");
            Views.MainWindow.StatusLabelColour.OnNext(orange);
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
            Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nERROR: Failed to establish connection to device.");
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
                Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nSuccessful connection confirmed");
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
                Views.MainWindow.StatusLabelText.OnNext("Connected to " + DeviceName);
                Views.MainWindow.StatusLabelColour.OnNext(green);
                return;
            }
            if (messageText.Contains("FIRMWARE_VER"))
            {
                string[] split = messageText.Split(' ');
                DeviceFirmware = split[1];
                Views.MainWindow.FirmwareLabelText.OnNext(DeviceFirmware);
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
                Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nERROR: Device reported excessive frame failure, disconnecting...");
                HeartbeatTimer.Stop();
                HeartbeatTimer.Close();
                CloseCommunicationSocket(CommunicationSocket);
                return;
            }
            if (messageText.Contains("OTA_MODE_ACTIVE"))
            {
                Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nDevice has entered OTA mode.");
                Views.MainWindow.StatusLabelText.OnNext("Connected in OTA mode");
                Views.MainWindow.StatusLabelColour.OnNext(purple);
                DeviceMode = "OTA";
            }
            else
            {
                Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nInvalid command received from device: " + messageText);
            }
        }

        private static void BinaryReceive(MemoryStream ms)
        {
            //(int right_X, int right_Y, int left_X, int left_Y) = EyeTrackingFramework.detectEyes(ms.ToArray());
        }

        private static void OnHeartbeatFailure(object sender, ElapsedEventArgs e)
        {
            //heartbeat was failed to be received within 10 seconds...
            Views.MainWindow.ConsoleLog.OnNext(Views.MainWindow.Console.Text + "\nERROR: Device timed out, Disconnecting...");
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
