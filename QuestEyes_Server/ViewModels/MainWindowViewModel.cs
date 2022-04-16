using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using ReactiveUI;

namespace QuestEyes_Server.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string CurrentStatus = "Searching...";
        private string CurrentStatusColour = "Red";
        private string CurrentBatteryStatus = "Not connected";
        private string CurrentFirmwareVersion = "Not connected";

        //stats text
        public string Status
        {
            get => CurrentStatus;
            set => this.RaiseAndSetIfChanged(ref CurrentStatus, value);
        }
        public string StatusColour
        {
            get => CurrentStatusColour;
            set => this.RaiseAndSetIfChanged(ref CurrentStatusColour, value);
        }
        public string BatteryStatus
        {
            get => CurrentBatteryStatus;
            set => this.RaiseAndSetIfChanged(ref CurrentBatteryStatus, value);
        }
        public string FirmwareVersion
        {
            get => CurrentFirmwareVersion;
            set => this.RaiseAndSetIfChanged(ref CurrentFirmwareVersion, value);
        }
    }
}
