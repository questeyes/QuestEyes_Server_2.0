using QuestEyes_Server.ViewModels;
using Avalonia.ReactiveUI;

namespace QuestEyes_Server.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}