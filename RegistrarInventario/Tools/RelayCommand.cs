using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RegistrarInventario
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> RelayExecute;

        public RelayCommand(Action<object> executeMethod) => RelayExecute = executeMethod;

        public RelayCommand()
        {
        }

        public bool CanExecute(object parameter) => true;  //Siempre true ya que no manejo inhabilitación de botones

        public void Execute(object parameter) => RelayExecute(parameter);
    }
}
