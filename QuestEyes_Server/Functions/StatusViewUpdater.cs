using Avalonia.Controls;
using Avalonia.Media;
using System.Reactive.Subjects;

namespace QuestEyes_Server.Functions
{
    public static class StatusViewUpdater
    {
        public static bool IsActive { get; set; } = false;
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

            ConsoleLog.OnNext(ConsoleContent);
            ConsoleCaret.OnNext(Console.Text.Length + 256);
        }
        public static void SetStatus(string mode, string? devicename)
        {
            switch (mode)
            {
                case "connecting":
                    StatusLabelText.OnNext("Connecting...");
                    StatusLabelColour.OnNext(orange);
                    break;
                case "connected":
                    StatusLabelText.OnNext("Connected to " + devicename);
                    StatusLabelColour.OnNext(green);
                    break;
                case "ota":
                    StatusLabelText.OnNext("Connected in OTA mode");
                    StatusLabelColour.OnNext(purple);
                    break;
                default:
                    StatusLabelText.OnNext("Searching...");
                    StatusLabelColour.OnNext(red);
                    break;
            }
        }

        public static void SetBatteryText(string message)
        {
            BatteryLabelText.OnNext(message);
        }
        public static void SetFirmwareText(string message)
        {
            FirmwareLabelText.OnNext(message);
        }

        public static void EnableButtons()
        {
            ButtonState.OnNext(true);
        }
        public static void DisableButtons()
        {
            ButtonState.OnNext(false);
        }
    }
}
