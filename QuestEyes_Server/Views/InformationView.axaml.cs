using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QuestEyes_Server.Views
{
    public partial class InformationView : UserControl
    {
        public InformationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
