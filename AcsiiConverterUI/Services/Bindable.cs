using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsciiConverterUI.Services
{
    public class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propName = null)
        {
            // Exit if no change is needed
            if (Equals(storage, value))
                return;

            // Set the property and notify listeners
            storage = value;
            PropChanged(propName);
        }
        protected void PropChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
