using System;
using System.Windows.Input;

namespace Library.Design.Command
{
    public class RelayCommand : ICommand
    {
        #region Event
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        #endregion

        #region Field
        private readonly Action execute;
        private readonly Func<bool> canExecute;
        #endregion

        #region Constructor
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }
        #endregion

        #region Method
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            execute();
        }
        #endregion
    }

    class RelayCommand<T> : ICommand
    {
        #region Event
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        #endregion

        #region Field
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;
        #endregion

        #region Constructor
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }
        #endregion

        #region Method
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
        #endregion
    }
}
