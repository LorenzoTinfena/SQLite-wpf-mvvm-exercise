using System;
using System.Windows.Input;

namespace WpfDotNetMvvmTemplate.ViewModels
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> OnExecute;
        private Predicate<object> OnCanExecute;

        public RelayCommand(Action<object> OnExecute, Predicate<object> OnCanExecute)
        {
            this.OnExecute = OnExecute;
            this.OnCanExecute = OnCanExecute;
        }
        public RelayCommand(Action<object> OnExecute)
        {
            this.OnExecute = OnExecute;
        }

        public bool CanExecute(object parameter)
        {
            return (OnCanExecute == null) ? true : OnCanExecute(parameter);
        }
        public void Execute(object parameter)
        {
            OnExecute?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
