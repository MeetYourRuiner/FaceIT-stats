using FaceitStats.WPF.ViewModels.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceitStats.WPF.Classes
{
    class VMFactory
    {
        private readonly List<Type> _store = new List<Type>();
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public VMFactory() { }

        private object ResolveType(Type serviceType)
        {
            if (!serviceType.IsInterface)
                return null;
            var service = _services.FirstOrDefault(s => s.Key == serviceType).Value;
            if (service == null)
                throw new Exception($"{serviceType.Name} was not registered.");
            return service;
        }

        public void AddViewModel<T>() where T : BaseViewModel
        {
            _store.Add(typeof(T));
        }

        public void AddService<T>(object implementation)
        {
            _services.Add(typeof(T), implementation);
        }

        public T Create<T>(object parameter = null) where T : BaseViewModel
        {
            var vmType = _store.FirstOrDefault((vm) => vm == typeof(T));
            var ctor = vmType.GetConstructors().FirstOrDefault();
            var ctorParams = ctor.GetParameters();
            object[] args = new object[ctorParams.Length];
            int i = 0;
            foreach (var param in ctorParams)
            {
                var service = ResolveType(param.ParameterType);
                if (service != null)
                    args[i++] = service;
            }
            if (i == ctorParams.Length - 1)
                args[i] = parameter;
            T vm = (T)ctor.Invoke(args);
            return vm;
        }
    }
}
