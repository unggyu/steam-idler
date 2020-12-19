using CommonServiceLocator;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace SteamIdler
{
    public class MSDIServiceLocator : IServiceLocator
    {
        public MSDIServiceLocator(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }

        public IServiceProvider ServiceProvider { get; }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotSupportedException();
        }

        public object GetInstance(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return ServiceProvider.GetService<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return ServiceProvider.GetService<TService>();
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }
    }
}
