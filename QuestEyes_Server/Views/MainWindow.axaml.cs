using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace QuestEyes_Server.Views
{
    public partial class MainWindow : Window
    {
        public static Subject<string> StatusLabelText { get; set; } = new Subject<string>();
        public static Subject<IBrush> StatusLabelColour { get; set; } = new Subject<IBrush>();
        public static Subject<string> BatteryLabelText { get; set; } = new Subject<string>();
        public static Subject<string> FirmwareLabelText { get; set; } = new Subject<string>();
        public static Subject<string> ConsoleLog { get; set; } = new Subject<string>();
        public static TextBox Console { get; set; } = default!;

        public MainWindow()
        {
            InitializeComponent();
            SetDefaultLabels();
            _ = Start();
        }

        public static async Task Start()
        {
            await Functions.DeviceConnectivity.SetupAndSearch();
        }

        public void SetDefaultLabels()
        {
            Console = consoleBox;

            var _statusLabel = statusLabel;
            _statusLabel.Bind(Label.ContentProperty, StatusLabelText);
            _statusLabel.Bind(Label.ForegroundProperty, StatusLabelColour);

            var _batteryLabel = batteryLabel;
            _batteryLabel.Bind(Label.ContentProperty, BatteryLabelText);

            var _firmwareLabel = firmwareLabel;
            _firmwareLabel.Bind(Label.ContentProperty, FirmwareLabelText);

            var _console = consoleBox;
            _console.Bind(TextBox.TextProperty, ConsoleLog);

            SolidColorBrush red = new();
            red.Color = Color.FromRgb(255, 0, 0);
            StatusLabelText.OnNext("Searching...");
            StatusLabelColour.OnNext(red);

            BatteryLabelText.OnNext("Not connected");
            FirmwareLabelText.OnNext("Not connected");

            ConsoleLog.OnNext("Welcome to the QuestEyes PC App!");
        }

        public void FirmwareUpdateCheckButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void ForceReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void FactoryResetButton_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotSupportedException();
        }
        public void DiagnosticsButton_Click(object sender, RoutedEventArgs e)
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
    }
}
