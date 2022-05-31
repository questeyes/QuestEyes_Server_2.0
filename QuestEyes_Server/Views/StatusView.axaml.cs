using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI;
using System.Diagnostics;

namespace QuestEyes_Server.Views
{
    public partial class StatusView : UserControl
    {
        public StatusView()
        {
            InitializeComponent();
            if (!Functions.StatusViewUpdater.IsActive) //if first time creating view...
            {
                SetControls();
                Functions.StatusViewUpdater.SetStatus("searching", null);
                Functions.StatusViewUpdater.SetBatteryText("Not connected");
                Functions.StatusViewUpdater.SetFirmwareText("Not connected");
                Functions.StatusViewUpdater.PrintToConsole("Welcome to the QuestEyes PC App!");
                _ = Start();
            } else //if view was made before...
            {
                SetControls();
                Functions.StatusViewUpdater.PrintToConsole("Refreshed information from when stats were hidden.");
                RefreshInfo();
            }
        }

        public static async Task Start()
        {
            await Functions.DeviceConnectivity.SetupAndSearch();
        }

        public void SetControls()
        {
            Functions.StatusViewUpdater.Console = consoleBox;

            Functions.StatusViewUpdater.red.Color = Color.FromRgb(255, 0, 0);
            Functions.StatusViewUpdater.orange.Color = Color.FromRgb(255, 165, 0);
            Functions.StatusViewUpdater.green.Color = Color.FromRgb(25, 145, 71);
            Functions.StatusViewUpdater.purple.Color = Color.FromRgb(138, 43, 226);

            var _statusLabel = statusLabel;
            _statusLabel.Bind(Label.ContentProperty, Functions.StatusViewUpdater.StatusLabelText);
            _statusLabel.Bind(Label.ForegroundProperty, Functions.StatusViewUpdater.StatusLabelColour);

            var _batteryLabel = batteryLabel;
            _batteryLabel.Bind(Label.ContentProperty, Functions.StatusViewUpdater.BatteryLabelText);

            var _firmwareLabel = firmwareLabel;
            _firmwareLabel.Bind(Label.ContentProperty, Functions.StatusViewUpdater.FirmwareLabelText);

            var _console = consoleBox;
            _console.Bind(TextBox.TextProperty, Functions.StatusViewUpdater.ConsoleLog);
            _console.Bind(TextBox.CaretIndexProperty, Functions.StatusViewUpdater.ConsoleCaret);

            var _FirmwareUpdateCheckButton = FirmwareUpdateCheckButton;
            var _ForceReconnectButton = ForceReconnectButton;
            var _FactoryResetButton = FactoryResetButton;
            var _DiagnosticsButton = DiagnosticsButton;
            var _CalibrateButton = CalibrateButton;
            _FirmwareUpdateCheckButton.Bind(Button.IsEnabledProperty, Functions.StatusViewUpdater.ButtonState);
            _ForceReconnectButton.Bind(Button.IsEnabledProperty, Functions.StatusViewUpdater.ButtonState);
            _FactoryResetButton.Bind(Button.IsEnabledProperty, Functions.StatusViewUpdater.ButtonState);
            _DiagnosticsButton.Bind(Button.IsEnabledProperty, Functions.StatusViewUpdater.ButtonState);
            _CalibrateButton.Bind(Button.IsEnabledProperty, Functions.StatusViewUpdater.ButtonState);

            Functions.StatusViewUpdater.IsActive = true;
        }
        public static void RefreshInfo()
        {
            if (Functions.DeviceConnectivity.Connected)
            {
                Functions.StatusViewUpdater.SetStatus("connected", Functions.DeviceConnectivity.DeviceName);
                Functions.StatusViewUpdater.SetBatteryText("Not connected");
                Functions.StatusViewUpdater.SetFirmwareText(Functions.DeviceConnectivity.DeviceFirmware);
                Functions.StatusViewUpdater.EnableButtons();
            }
            else if (Functions.DeviceConnectivity.AttemptingConnection)
            {
                Functions.StatusViewUpdater.SetStatus("connecting", null);
                Functions.StatusViewUpdater.SetBatteryText("Not connected");
                Functions.StatusViewUpdater.SetFirmwareText("Not connected");
                Functions.StatusViewUpdater.DisableButtons();
            }
            else
            {
                Functions.StatusViewUpdater.SetStatus("searching...", null);
                Functions.StatusViewUpdater.SetBatteryText("Not connected");
                Functions.StatusViewUpdater.SetFirmwareText("Not connected");
                Functions.StatusViewUpdater.DisableButtons();
            }
        }


        public void ForceReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.StatusViewUpdater.PrintToConsole("Forcing reconnect per user request...");
            Functions.DeviceConnectivity.HeartbeatTimer.Stop();
            Functions.DeviceConnectivity.HeartbeatTimer.Close();
            Functions.DeviceConnectivity.CloseCommunicationSocket(Functions.DeviceConnectivity.CommunicationSocket);
        }
        public void OscControlButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void CalibrateButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void UpdateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void DiagnosticsButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
    }
}
