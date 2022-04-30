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
        private string _serverVersion = "QuestEyes PC App v0.0.0";

        public string ServerVersion
        {
            get => _serverVersion;
            set => this.RaiseAndSetIfChanged(ref _serverVersion, value);
        }

        public MainWindowViewModel()
        {
            ServerVersion = $"QuestEyes PC App v{string.Format(CultureInfo.InvariantCulture, @"{0}.{1}.{2}", CoreAssembly.Version.Major, CoreAssembly.Version.Minor, CoreAssembly.Version.Build)}";
        }
    }
}
