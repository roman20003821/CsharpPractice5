using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharpPractice5.Tools
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {
        public abstract void OnReOpen();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
