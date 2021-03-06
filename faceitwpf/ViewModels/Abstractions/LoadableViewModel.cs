﻿using faceitwpf.ViewModels.Commands;
using System.Threading.Tasks;

namespace faceitwpf.ViewModels.Abstractions
{
    public abstract class LoadableViewModel : BaseViewModel
    {
        public bool isLoaded { get; set; }

        public bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        protected LoadedCommand _loadedCommand;
        public LoadedCommand LoadedCommand
        {
            get => _loadedCommand ?? (_loadedCommand = new LoadedCommand(async (obj) =>
            {
                await LoadedMethod(obj);
            }, this));
        }

        public abstract Task LoadedMethod(object obj);
    }
}
