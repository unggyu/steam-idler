using ModernWpf.Controls;
using SteamIdler.Properties;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SteamIdler.Services
{
    public interface IDialogService
    {
        Task<bool> ShowDeleteAccountDialogAsync();
    }

    public class DialogService : IDialogService
    {
        public async Task<bool> ShowDeleteAccountDialogAsync()
        {
            var primaryButtonStyle = new Style
            {
                TargetType = typeof(Button),
                BasedOn = (Style)App.Current.Resources[typeof(Button)]
            };
            primaryButtonStyle.Setters.Add(new Setter
            {
                Property = Control.BackgroundProperty,
                Value = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
            });

            var dialog = new ContentDialog
            {
                Title = Resources.RemoveAccount,
                Content = Resources.AccountDeletionConfirmationPhrase,
                PrimaryButtonText = Resources.Delete,
                PrimaryButtonStyle = primaryButtonStyle,
                CloseButtonText = Resources.Cancel
            };

            var result = await dialog.ShowAsync();

            return result == ContentDialogResult.Primary;
        }
    }
}
