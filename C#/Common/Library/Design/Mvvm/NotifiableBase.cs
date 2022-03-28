using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Library.Design.Mvvm
{
    [DebuggerStepThrough]
    public class NotifiableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}