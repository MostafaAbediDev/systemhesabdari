using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Taadol.Controls;
using Taadol.ViewModels;

namespace Taadol.Views
{
    public partial class NewProductView : UserControl
    {
        public NewProductViewModel ViewModel { get; }

        public NewProductView()
        {
            InitializeComponent();
            ViewModel = new NewProductViewModel();
            DataContext = ViewModel;

            Loaded += (s, e) => UpdateDividerEllipseColor();
        }

        private void UpdateDividerEllipseColor()
        {
            bool isModal = Window.GetWindow(this) is MainWindow mw && mw.ModalContent.Content == this;
            var colorHex = isModal ? "#66000000" : "#FFF8ED";
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex));
            DividerEllipseLeft.Fill = brush;
            DividerEllipseRight.Fill = brush;
        }
        private void RootBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RootBorder.Clip = new RectangleGeometry(
                new Rect(0, 0, e.NewSize.Width, e.NewSize.Height), 4, 4);
        }
        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;
            if (mainWindow.ModalContent.Content == this)
                mainWindow.CloseModal();
            else
                mainWindow.CloseCurrentForm();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;
            if (mainWindow.ModalContent.Content == this)
                mainWindow.CloseModal();
            else
                mainWindow.CloseCurrentForm();
        }

        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
            var imagePicker = sender as ImagePickerControl;
            if (imagePicker != null)
            {
                string imagePath = imagePicker.ImagePath;
            }
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
        }

        private void TabInfo_Checked(object sender, RoutedEventArgs e)
        {
            if (InfoContent != null) InfoContent.Visibility = Visibility.Visible;
            if (VariantsContent != null) VariantsContent.Visibility = Visibility.Collapsed;
            if (PricingContent != null) PricingContent.Visibility = Visibility.Collapsed;
        }

        private void TabVariants_Checked(object sender, RoutedEventArgs e)
        {
            if (InfoContent != null) InfoContent.Visibility = Visibility.Collapsed;
            if (VariantsContent != null) VariantsContent.Visibility = Visibility.Visible;
            if (PricingContent != null) PricingContent.Visibility = Visibility.Collapsed;
        }

        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (InfoContent != null) InfoContent.Visibility = Visibility.Collapsed;
            if (VariantsContent != null) VariantsContent.Visibility = Visibility.Collapsed;
            if (PricingContent != null) PricingContent.Visibility = Visibility.Visible;
        }

        private void CodeModeToggle_SelectionChanged(object sender, bool e)
        {
        }

        private void View_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
