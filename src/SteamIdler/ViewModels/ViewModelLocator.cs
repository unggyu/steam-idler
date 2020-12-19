﻿using CommonServiceLocator;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SteamIdler.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelLocator()
        {
            _serviceProvider = ServiceLocator.Current.GetService<IServiceProvider>();
        }

        public MySplashViewModel MySplash => _serviceProvider.GetService<MySplashViewModel>();
        public LoginViewModel Login => _serviceProvider.GetService<LoginViewModel>();
    }
}
