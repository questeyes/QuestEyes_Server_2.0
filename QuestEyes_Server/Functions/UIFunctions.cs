using Avalonia.Media;
using System.Threading.Tasks;

namespace QuestEyes_Server.Functions
{
    public static class UIFunctions
    {
        public static string? ConsoleContent { get; set; }

        public static void PrintToConsole(string message)
        {
            ConsoleContent = ConsoleContent + message + "\n";

            Views.MainWindow.ConsoleLog.OnNext(ConsoleContent);
            Views.MainWindow.ConsoleCaret.OnNext(Views.MainWindow.Console.Text.Length + 256);
        }
        public static void SetStatus(string mode, string? devicename)
        {
            switch (mode)
            {
                case "connecting":
                    Views.MainWindow.StatusLabelText.OnNext("Connecting...");
                    Views.MainWindow.StatusLabelColour.OnNext(Views.MainWindow.orange);
                    break;
                case "connected":
                    Views.MainWindow.StatusLabelText.OnNext("Connected to " + devicename);
                    Views.MainWindow.StatusLabelColour.OnNext(Views.MainWindow.green);
                    break;
                case "ota":
                    Views.MainWindow.StatusLabelText.OnNext("Connected in OTA mode");
                    Views.MainWindow.StatusLabelColour.OnNext(Views.MainWindow.purple);
                    break;
                default:
                    Views.MainWindow.StatusLabelText.OnNext("Searching...");
                    Views.MainWindow.StatusLabelColour.OnNext(Views.MainWindow.red);
                    break;
            }
        }

        public static void SetBatteryText(string message)
        {
            Views.MainWindow.BatteryLabelText.OnNext(message);
        }
        public static void SetFirmwareText(string message)
        {
            Views.MainWindow.FirmwareLabelText.OnNext(message);
        }
    }
}
