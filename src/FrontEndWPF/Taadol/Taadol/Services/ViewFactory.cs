using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Taadol.Services
{
    public class ViewFactory
    {
        private readonly Dictionary<string, Func<UserControl>> _registry = new Dictionary<string, Func<UserControl>>();
        private readonly IServiceProvider _serviceProvider;

        // Constructor رو به این شکل تغییر دادم تا همServiceProvider بگیره هم ارور نده
        public ViewFactory(IServiceProvider serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
        }

        public void Register(string key, Func<UserControl> creator)
        {
            _registry[key] = creator;
        }

        public UserControl Create(string key)
        {
            if (!_registry.ContainsKey(key))
            {
                MessageBox.Show("صفحه مورد نظر پیدا نشد: " + key);
                return null;
            }

            return _registry[key].Invoke();
        }
    }
}