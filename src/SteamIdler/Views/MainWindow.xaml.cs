using SteamIdler.Infrastructure.Helpers;
using System.Windows;
using System.Windows.Input;

namespace SteamIdler.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AppIdTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexHelper.IsNumeric(e.Text);
        }
    }
}
