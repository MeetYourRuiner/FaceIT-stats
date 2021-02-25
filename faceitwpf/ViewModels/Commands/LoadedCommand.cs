using faceitwpf.ViewModels.Abstractions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace faceitwpf.ViewModels.Commands
{
    public class LoadedCommand : ICommand
    {
        private Func<object, Task> execute;
        private LoadableViewModel viewModel;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public LoadedCommand(Func<object, Task> execute, LoadableViewModel viewModel)
        {
            this.execute = execute;
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return !this.viewModel.isLoaded;
        }

        public async void Execute(object parameter)
        {
            viewModel.IsLoading = true;
            await this.execute(parameter);
            viewModel.IsLoading = false;
            viewModel.isLoaded = true;
        }
    }
}
