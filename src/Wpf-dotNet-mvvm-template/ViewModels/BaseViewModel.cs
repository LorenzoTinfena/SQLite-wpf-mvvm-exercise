using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SQLiteWpfMvvmExercise.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
