using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Taadol.Controls
{
    public partial class ImagePickerControl : UserControl
    {
                private static readonly Color _addBorder = (Color)ColorConverter.ConvertFromString("#2667FF");
        private static readonly Color _addBorderHover = (Color)ColorConverter.ConvertFromString("#1A4FCC");
        private static readonly Color _addBgHover = (Color)ColorConverter.ConvertFromString("#EEF3FF");
        private static readonly Color _addText = (Color)ColorConverter.ConvertFromString("#2667FF");

        private static readonly Color _removeBorder = (Color)ColorConverter.ConvertFromString("#FF4D4F");
        private static readonly Color _removeBorderHover = (Color)ColorConverter.ConvertFromString("#CC1A1C");
        private static readonly Color _removeBgHover = (Color)ColorConverter.ConvertFromString("#FFEBEB");
        private static readonly Color _removeText = (Color)ColorConverter.ConvertFromString("#FF4D4F");

        private static readonly Color _imgBorderActive = (Color)ColorConverter.ConvertFromString("#A5D6A7");
        private static readonly Color _imgBorderDefault = (Color)ColorConverter.ConvertFromString("#BED1FF");
        private static readonly Color _buttonBgDefault = Colors.White;

        private bool _hasImage = false;

                        
        public static readonly DependencyProperty ImagePathProperty =
          DependencyProperty.Register(
              "ImagePath", typeof(string), typeof(ImagePickerControl),
              new FrameworkPropertyMetadata(null,
                  FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                  OnImagePathChanged));

                public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }
        public static readonly DependencyProperty PlaceholderIconSourceProperty =
            DependencyProperty.Register(
                "PlaceholderIconSource", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("/Assets/Icons/profile-circle.svg", OnPlaceholderIconChanged));

        public static readonly DependencyProperty AddIconSourceProperty =
            DependencyProperty.Register(
                "AddIconSource", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("/Assets/Icons/add-square.svg"));

        public static readonly DependencyProperty RemoveIconSourceProperty =
            DependencyProperty.Register(
                "RemoveIconSource", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("/Assets/Icons/trash.svg"));

        public static readonly DependencyProperty AddLabelProperty =
            DependencyProperty.Register(
                "AddLabel", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("جدید", OnLabelChanged));

        public static readonly DependencyProperty RemoveLabelProperty =
            DependencyProperty.Register(
                "RemoveLabel", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("حذف", OnLabelChanged));

        
      
        public string PlaceholderIconSource
        {
            get { return (string)GetValue(PlaceholderIconSourceProperty); }
            set { SetValue(PlaceholderIconSourceProperty, value); }
        }

        public string AddIconSource
        {
            get { return (string)GetValue(AddIconSourceProperty); }
            set { SetValue(AddIconSourceProperty, value); }
        }

        public string RemoveIconSource
        {
            get { return (string)GetValue(RemoveIconSourceProperty); }
            set { SetValue(RemoveIconSourceProperty, value); }
        }

        public string AddLabel
        {
            get { return (string)GetValue(AddLabelProperty); }
            set { SetValue(AddLabelProperty, value); }
        }

        public string RemoveLabel
        {
            get { return (string)GetValue(RemoveLabelProperty); }
            set { SetValue(RemoveLabelProperty, value); }
        }

                        
        public static readonly RoutedEvent ImageSelectedEvent =
     EventManager.RegisterRoutedEvent(
         "ImageSelected",
         RoutingStrategy.Bubble,
         typeof(RoutedEventHandler),           typeof(ImagePickerControl));

        public static readonly RoutedEvent ImageRemovedEvent =
            EventManager.RegisterRoutedEvent(
                "ImageRemoved",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ImagePickerControl));

        public event RoutedEventHandler ImageSelected
        {
            add { AddHandler(ImageSelectedEvent, value); }
            remove { RemoveHandler(ImageSelectedEvent, value); }
        }

        public event RoutedEventHandler ImageRemoved
        {
            add { AddHandler(ImageRemovedEvent, value); }
            remove { RemoveHandler(ImageRemovedEvent, value); }
        }

                        
        public ImagePickerControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                ApplyPlaceholderIcon();
                UpdateState(animate: false);
            };
        }

                        
        private void ActionButton_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"🖱️ Button clicked, _hasImage = {_hasImage}");

            if (_hasImage)
            {
                System.Diagnostics.Debug.WriteLine("🗑️ حذف تصویر");
                RemoveImage();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("📁 باز کردن فایل");
                OpenImagePicker();
            }
        }

        private void ActionButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_hasImage)
            {
                                AnimateBorderColor(ActionButton, _removeBgHover, _removeBorderHover);

            }
            else
            {
                                AnimateBorderColor(ActionButton, _addBgHover, _addBorderHover);

            }
        }

        private void ActionButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetButtonColors();
        }

                        
        private void OpenImagePicker()
        {
            var dialog = new OpenFileDialog
            {
                Title = "انتخاب تصویر",
                Filter = "تصاویر|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|همه فایل‌ها|*.*",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
                ImagePath = dialog.FileName;
        }

        private void RemoveImage()
        {
            ImagePath = null;
        }
        private void LoadImage(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 400;
                bitmap.EndInit();
                bitmap.Freeze();

                PreviewImage.Source = bitmap;
                PreviewImage.Stretch = Stretch.Uniform;

                _hasImage = true;              }
            catch (Exception)
            {
                            }
        }
                                        private void UpdateState(bool animate = true)
        {
            var duration = animate
                ? TimeSpan.FromSeconds(0.22)
                : TimeSpan.Zero;

            if (_hasImage)
            {
                                PlaceholderPanel.Visibility = Visibility.Collapsed;
                PreviewImage.Visibility = Visibility.Visible;

                
                SetSvg(ButtonIcon, RemoveIconSource);

                                SetBorderColors(ActionButton, _buttonBgDefault, _removeBorder);


                                AnimateBorderStroke(ImageBorder, _imgBorderActive, duration);
            }
            else
            {
                                PreviewImage.Visibility = Visibility.Collapsed;
                PreviewImage.Source = null;
                PlaceholderPanel.Visibility = Visibility.Visible;

                
                SetSvg(ButtonIcon, AddIconSource);

                                SetBorderColors(ActionButton, _buttonBgDefault, _addBorder);

                                AnimateBorderStroke(ImageBorder, _imgBorderDefault, duration);
            }
        }

                        
        private void SetSvg(SharpVectors.Converters.SvgViewbox box, string source)
        {
            if (box == null || string.IsNullOrEmpty(source)) return;
            try { box.Source = new Uri(source, UriKind.RelativeOrAbsolute); }
            catch { }
        }

        private void ApplyPlaceholderIcon()
        {
            SetSvg(PlaceholderIcon, PlaceholderIconSource);
        }

                        
                private void SetBorderColors(Border border, Color bg, Color stroke)
        {
            if (border == null) return;
            border.Background = new SolidColorBrush(bg);
            border.BorderBrush = new SolidColorBrush(stroke);
        }

                private void AnimateBorderColor(Border border, Color bgTo, Color strokeTo)
        {
            if (border == null) return;

            var dur = new Duration(TimeSpan.FromSeconds(0.15));

                        Color currentBg = Colors.Transparent;
            if (border.Background is SolidColorBrush bgBrush)
                currentBg = bgBrush.Color;

            var newBg = new SolidColorBrush(currentBg);               border.Background = newBg;
            newBg.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(bgTo, dur));

                        Color currentStroke = Colors.Transparent;
            if (border.BorderBrush is SolidColorBrush strokeBrush)
                currentStroke = strokeBrush.Color;

            var newStroke = new SolidColorBrush(currentStroke);              border.BorderBrush = newStroke;
            newStroke.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(strokeTo, dur));
        }

                private void AnimateBorderStroke(Border border, Color to, TimeSpan? duration = null)
        {
            if (border == null) return;

            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.22));

            Color current = Colors.Transparent;
            if (border.BorderBrush is SolidColorBrush existing)
                current = existing.Color;

            var newBrush = new SolidColorBrush(current);              border.BorderBrush = newBrush;
            newBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(to, dur));
        }

                private void AnimateTextColor(TextBlock tb, Color to, TimeSpan? duration = null)
        {
            if (tb == null) return;

            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.15));

            Color current = Colors.Black;
            if (tb.Foreground is SolidColorBrush existing)
                current = existing.Color;

            var newBrush = new SolidColorBrush(current);              tb.Foreground = newBrush;
            newBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(to, dur));
        }

                private void ResetButtonColors()
        {
            if (_hasImage)
            {
                AnimateBorderColor(ActionButton, _buttonBgDefault, _removeBorder);
            }
            else
            {
                AnimateBorderColor(ActionButton, _buttonBgDefault, _addBorder);
            }
        }

                        
        private static void OnImagePathChanged(DependencyObject d,
     DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ImagePickerControl;
            if (ctrl == null) return;

            var path = e.NewValue as string;

                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                ctrl.LoadImage(path);
                ctrl.UpdateState(animate: true);
                ctrl.RaiseEvent(new RoutedEventArgs(ImageSelectedEvent, ctrl));
            }
                        else
            {
                ctrl._hasImage = false;
                ctrl.PreviewImage.Source = null;                  ctrl.PreviewImage.Visibility = Visibility.Collapsed;
                ctrl.PlaceholderPanel.Visibility = Visibility.Visible;
                ctrl.UpdateState(animate: true);

                if (e.OldValue != null)
                    ctrl.RaiseEvent(new RoutedEventArgs(ImageRemovedEvent, ctrl));
            }
        }
        private static void OnPlaceholderIconChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ImagePickerControl;
            if (ctrl != null) ctrl.ApplyPlaceholderIcon();
        }

        private static void OnLabelChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ImagePickerControl;
            if (ctrl != null) ctrl.UpdateState(animate: false);
        }
    }
}
