namespace QuestEyes_Server.Functions
{
    public static class StatusViewControls
    {
        public static string? ConsoleContent { get; set; }

        public static void PrintToConsole(string message)
        {
            if (ConsoleContent == null)
            {
                ConsoleContent += message;
            }
            else {
                ConsoleContent += "\n" + message ;
            }

            Views.StatusView.ConsoleLog.OnNext(ConsoleContent);
            Views.StatusView.ConsoleCaret.OnNext(Views.StatusView.Console.Text.Length + 256);
        }
        public static void SetStatus(string mode, string? devicename)
        {
            switch (mode)
            {
                case "connecting":
                    Views.StatusView.StatusLabelText.OnNext("Connecting...");
                    Views.StatusView.StatusLabelColour.OnNext(Views.StatusView.orange);
                    break;
                case "connected":
                    Views.StatusView.StatusLabelText.OnNext("Connected to " + devicename);
                    Views.StatusView.StatusLabelColour.OnNext(Views.StatusView.green);
                    break;
                case "ota":
                    Views.StatusView.StatusLabelText.OnNext("Connected in OTA mode");
                    Views.StatusView.StatusLabelColour.OnNext(Views.StatusView.purple);
                    break;
                default:
                    Views.StatusView.StatusLabelText.OnNext("Searching...");
                    Views.StatusView.StatusLabelColour.OnNext(Views.StatusView.red);
                    break;
            }
        }

        public static void SetBatteryText(string message)
        {
            Views.StatusView.BatteryLabelText.OnNext(message);
        }
        public static void SetFirmwareText(string message)
        {
            Views.StatusView.FirmwareLabelText.OnNext(message);
        }

        public static void EnableButtons()
        {
            Views.StatusView.ButtonState.OnNext(true);
        }
        public static void DisableButtons()
        {
            Views.StatusView.ButtonState.OnNext(false);
        }
    }
}
