using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ReactiveUI;

namespace QuestEyes_Server.Views
{
    public partial class StatusView : UserControl
    {
        public static Subject<string> StatusLabelText { get; set; } = new Subject<string>();
        public static Subject<IBrush> StatusLabelColour { get; set; } = new Subject<IBrush>();
        public static Subject<string> BatteryLabelText { get; set; } = new Subject<string>();
        public static Subject<string> FirmwareLabelText { get; set; } = new Subject<string>();
        public static TextBox Console { get; set; } = default!;
        public static Subject<string> ConsoleLog { get; set; } = new Subject<string>();
        public static Subject<int> ConsoleCaret { get; set; } = new Subject<int>();
        public static Subject<bool> ButtonState { get; set; } = new Subject<bool>();

        //Colours for status label
        public static readonly SolidColorBrush red = new();
        public static readonly SolidColorBrush orange = new();
        public static readonly SolidColorBrush green = new();
        public static readonly SolidColorBrush purple = new();

        public StatusView()
        {
            InitializeComponent();
            SetControls();
            _ = Start();
        }

        public static async Task Start()
        {
            await Functions.DeviceConnectivity.SetupAndSearch();
        }

        public void SetControls()
        {
            Console = consoleBox;

            red.Color = Color.FromRgb(255, 0, 0);
            orange.Color = Color.FromRgb(255, 165, 0);
            green.Color = Color.FromRgb(25, 145, 71);
            purple.Color = Color.FromRgb(138, 43, 226);

            var _statusLabel = statusLabel;
            _statusLabel.Bind(Label.ContentProperty, StatusLabelText);
            _statusLabel.Bind(Label.ForegroundProperty, StatusLabelColour);

            var _batteryLabel = batteryLabel;
            _batteryLabel.Bind(Label.ContentProperty, BatteryLabelText);

            var _firmwareLabel = firmwareLabel;
            _firmwareLabel.Bind(Label.ContentProperty, FirmwareLabelText);

            var _console = consoleBox;
            _console.Bind(TextBox.TextProperty, ConsoleLog);
            _console.Bind(TextBox.CaretIndexProperty, ConsoleCaret);

            var _FirmwareUpdateCheckButton = FirmwareUpdateCheckButton;
            var _ForceReconnectButton = ForceReconnectButton;
            var _FactoryResetButton = FactoryResetButton;
            var _DiagnosticsButton = DiagnosticsButton;
            var _CalibrateButton = CalibrateButton;
            _FirmwareUpdateCheckButton.Bind(Button.IsEnabledProperty, ButtonState);
            _ForceReconnectButton.Bind(Button.IsEnabledProperty, ButtonState);
            _FactoryResetButton.Bind(Button.IsEnabledProperty, ButtonState);
            _DiagnosticsButton.Bind(Button.IsEnabledProperty, ButtonState);
            _CalibrateButton.Bind(Button.IsEnabledProperty, ButtonState);

            Functions.StatusViewControls.SetStatus("searching", null);
            Functions.StatusViewControls.SetBatteryText("Not connected");
            Functions.StatusViewControls.SetFirmwareText("Not connected");
            Functions.StatusViewControls.PrintToConsole("Welcome to the QuestEyes PC App!");
        }
        public void ForceReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.StatusViewControls.PrintToConsole("Forcing reconnect per user request...");
            Functions.DeviceConnectivity.HeartbeatTimer.Stop();
            Functions.DeviceConnectivity.HeartbeatTimer.Close();
            Functions.DeviceConnectivity.CloseCommunicationSocket(Functions.DeviceConnectivity.CommunicationSocket);
        }
        public void FactoryResetButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
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
