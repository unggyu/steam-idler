using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SteamIdler.Infrastructure
{
    public class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;

            RaisePropertyChanged(propertyName);
        }
    }
}
