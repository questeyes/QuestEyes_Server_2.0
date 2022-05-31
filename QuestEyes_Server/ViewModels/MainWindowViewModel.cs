using ReactiveUI;

namespace QuestEyes_Server.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;
        public MainWindowViewModel()
        {
            Content = Status = new StatusViewModel();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public StatusViewModel Status { get; }

        public void FactoryResetPage()
        {
            Content = new ResetViewModel();
        }
    }
}
