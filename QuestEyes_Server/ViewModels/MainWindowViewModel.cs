using ReactiveUI;

namespace QuestEyes_Server.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public StatusViewModel Status { get; }

        public MainWindowViewModel()
        {
            Content = Status = new StatusViewModel();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void ReturnToStats()
        {
            Content = Status;
        }

        public void FactoryResetPage()
        {
            Content = new ResetViewModel();
        }
    }
}
