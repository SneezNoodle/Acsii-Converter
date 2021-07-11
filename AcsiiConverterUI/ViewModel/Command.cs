using System;
using System.Windows.Input;

namespace AsciiConverterUI.ViewModel
{
    public class Command : ICommand
    {
        private readonly Action executeAction;

        public event EventHandler CanExecuteChanged;

        public Command(Action execute)
        {
            executeAction = execute;
        }

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => executeAction.Invoke();
    }
}
