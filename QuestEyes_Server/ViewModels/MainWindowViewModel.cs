using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive;
using System.Reflection;

namespace QuestEyes_Server.ViewModels
{
    public static class CoreAssembly
    {
        public static readonly Assembly Reference = typeof(CoreAssembly).Assembly;
        public static readonly Version Version = Reference.GetName().Version;
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private string _status = "Searching...";
        private string _statusColour = "Red";
        private string _batteryStatus = "Not connected";
        private string _firmwareVersion = "Not connected";
        private string _serverVersion = "QuestEyes PC App v0.0.0";

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
        public string StatusColour
        {
            get => _statusColour;
            set => this.RaiseAndSetIfChanged(ref _statusColour, value);
        }
        public string BatteryStatus
        {
            get => _batteryStatus;
            set => this.RaiseAndSetIfChanged(ref _batteryStatus, value);
        }
        public string FirmwareVersion
        {
            get => _firmwareVersion;
            set => this.RaiseAndSetIfChanged(ref _firmwareVersion, value);
        }
        public string ServerVersion
        {
            get => _serverVersion;
            set => this.RaiseAndSetIfChanged(ref _serverVersion, value);
        }

        public MainWindowViewModel()
        {
            ServerVersion = $"QuestEyes PC App v{string.Format(CultureInfo.InvariantCulture, @"{0}.{1}.{2}", CoreAssembly.Version.Major, CoreAssembly.Version.Minor, CoreAssembly.Version.Build)}";
        }

        public void _ChangeStatus(string status)//, string devicename)
        {
            switch(status)
            {
                case "searching":
                    Status = "Searching...";
                    StatusColour = "red";
                    break;

                case "connecting":
                    Status = "Connecting...";
                    StatusColour = "orange";
                    break;

                case "connected":
                    Status = "Connected to ";// + devicename;
                    StatusColour = "green";
                    break;

                case "ota":
                    Status = "Connected in OTA mode";
                    StatusColour = "purple";
                    break;
            }
        }

        public void _ChangeBatteryStatusDisplay(string _batteryPercentage)
        {
            if (_batteryPercentage == null)
            {
                BatteryStatus = "Not connected";
            } else
            {
                BatteryStatus = _batteryPercentage + "%";
            }
        }

        public void _ChangeFirmwareVersionDisplay(string _version)
        {
            if (_version == null)
            {
                FirmwareVersion = "Not connected";
            }
            else
            {
                FirmwareVersion = _version;
            }
        }
    }
}
