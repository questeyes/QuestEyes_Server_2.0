using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;

namespace QuestEyes_Server.ViewModels
{
    public static class CoreAssembly
    {
        public static readonly Assembly? Reference = typeof(CoreAssembly).Assembly;
        public static readonly Version? Version = Reference.GetName().Version;
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public Interaction<UpdaterWindowViewModel, ViewModelBase> UpdaterView { get; }
        public Interaction<DiagnosticsWindowViewModel, ViewModelBase> DiagnosticsView { get; }
        public ICommand ShowUpdater { get; }
        public ICommand ShowDiagnostics { get; }

        private string _serverVersion = "QuestEyes PC App (version unknown)";

        public string ServerVersion
        {
            get => _serverVersion;
            set => this.RaiseAndSetIfChanged(ref _serverVersion, value);
        }

        public MainWindowViewModel()
        {
            if (CoreAssembly.Version != null)
            {
                ServerVersion = $"QuestEyes PC App v{string.Format(CultureInfo.InvariantCulture, @"{0}.{1}.{2}", CoreAssembly.Version.Major, CoreAssembly.Version.Minor, CoreAssembly.Version.Build)}";
            }

            UpdaterView = new Interaction<UpdaterWindowViewModel, ViewModelBase>();
            ShowUpdater = ReactiveCommand.CreateFromTask(async () =>
            {
                var updater = new UpdaterWindowViewModel();
#pragma warning disable S1481 // Unused local variables should be removed
                var updaterDialog = await UpdaterView.Handle(updater);
#pragma warning restore S1481 // Unused local variables should be removed
            });

            DiagnosticsView = new Interaction<DiagnosticsWindowViewModel, ViewModelBase>();
            ShowDiagnostics = ReactiveCommand.CreateFromTask(async () =>
            {
                var diagnostics = new DiagnosticsWindowViewModel();
#pragma warning disable S1481 // Unused local variables should be removed
                var diagnosticsDialog = await DiagnosticsView.Handle(diagnostics);
#pragma warning restore S1481 // Unused local variables should be removed
            });
        }
    }
}
