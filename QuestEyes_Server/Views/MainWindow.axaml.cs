using Avalonia.Controls;
using Avalonia.Interactivity;

namespace QuestEyes_Server.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Functions.DeviceConnectivity.Search();
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
