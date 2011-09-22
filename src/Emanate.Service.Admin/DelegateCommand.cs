using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Emanate.Service.Admin
{
    [DebuggerStepThrough]
    internal class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                execute();
            }
        }
    }

    [DebuggerStepThrough]
    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, bool> canExecute;
        private readonly Action<T> execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<T> execute)
            : this(execute, parameter => true)
        {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                execute((T)parameter);
            }
        }
    }
}