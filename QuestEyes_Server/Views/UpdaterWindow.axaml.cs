using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QuestEyes_Server.Views
{
    public partial class UpdaterWindow : Window
    {
        public UpdaterWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
