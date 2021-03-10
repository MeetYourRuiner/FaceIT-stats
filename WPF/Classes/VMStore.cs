using FaceitStats.WPF.ViewModels.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceitStats.WPF.Classes
{
    class VMStore
    {
        private readonly Dictionary<Type, Func<object, object>> _store = new Dictionary<Type, Func<object, object>>();

        public VMStore() { }

        public void Add<T>(Func<object, T> factoryMethod) where T : BaseViewModel
        {
            _store.Add(typeof(T), factoryMethod);
        }

        public T Get<T>(object parameter) where T : BaseViewModel
        {
            var vm = _store.FirstOrDefault((o) => o.Key == typeof(T));
            return vm.Value?.Invoke(parameter) as T;
        }
    }
}
