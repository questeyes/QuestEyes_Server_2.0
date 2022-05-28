using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QuestEyes_Server.Views
{
    public partial class DiagnosticsWindow : Window
    {
        public DiagnosticsWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
