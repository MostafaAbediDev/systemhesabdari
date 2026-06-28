using System;
using System.Windows;
using System.Windows.Controls;

namespace Taadol.Services
{
    public class NavigationService
    {
        private readonly ContentControl _content;
        private readonly ViewFactory _factory;

        public NavigationService(ContentControl content, ViewFactory factory)
        {
            _content = content;
            _factory = factory;
        }

        public void Navigate(string key)
        {
            try
            {
                var view = _factory.Create(key);

                if (view == null)
                    return;

                _content.Content = view;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation Error: {ex.Message}");
            }
        }
    }
}