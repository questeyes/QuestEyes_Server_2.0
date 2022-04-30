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
        public static TextBox Console { get; set; } = default!;
        public static Subject<string> ConsoleLog { get; set; } = new Subject<string>();
        public static Subject<int> ConsoleCaret { get; set; } = new Subject<int>();



        //Colours for status label
        public static readonly SolidColorBrush red = new();
        public static readonly SolidColorBrush orange = new();
        public static readonly SolidColorBrush green = new();
        public static readonly SolidColorBrush purple = new();

        public MainWindow()
        {
            InitializeComponent();
            SetControls();
            _ = Start();
        }

        public static async Task Start()
        {
            await Models.DeviceConnectivity.SetupAndSearch();
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

            Functions.UIFunctions.SetStatus("searching", null);
            Functions.UIFunctions.SetBatteryText("Not connected");
            Functions.UIFunctions.SetFirmwareText("Not connected");
            Functions.UIFunctions.PrintToConsole("Welcome to the QuestEyes PC App!");
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
