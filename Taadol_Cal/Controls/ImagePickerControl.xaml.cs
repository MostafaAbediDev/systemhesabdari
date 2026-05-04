using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Taadol_Cal.Controls
{
    public partial class ImagePickerControl : UserControl
    {
        // ── رنگ‌های ثابت ──────────────────────────────────────
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

        // ══════════════════════════════════════════════════════
        //  Dependency Properties
        // ══════════════════════════════════════════════════════

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(
                "ImagePath", typeof(string), typeof(ImagePickerControl),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnImagePathChanged));

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
                new PropertyMetadata("/Assets/Icons/close.svg"));

        public static readonly DependencyProperty AddLabelProperty =
            DependencyProperty.Register(
                "AddLabel", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("جدید", OnLabelChanged));

        public static readonly DependencyProperty RemoveLabelProperty =
            DependencyProperty.Register(
                "RemoveLabel", typeof(string), typeof(ImagePickerControl),
                new PropertyMetadata("حذف", OnLabelChanged));

        // ── CLR Wrappers ──────────────────────────────────────

        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

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

        // ══════════════════════════════════════════════════════
        //  Routed Events
        // ══════════════════════════════════════════════════════

        public static readonly RoutedEvent ImageSelectedEvent =
            EventManager.RegisterRoutedEvent(
                "ImageSelected",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ImagePickerControl));

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

        // ══════════════════════════════════════════════════════
        //  Constructor
        // ══════════════════════════════════════════════════════

        public ImagePickerControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                ApplyPlaceholderIcon();
                UpdateState(animate: false);
            };
        }

        // ══════════════════════════════════════════════════════
        //  Event Handlers - UI
        // ══════════════════════════════════════════════════════

        private void ActionButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (_hasImage)
                RemoveImage();
            else
                OpenImagePicker();
        }

        private void ActionButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_hasImage)
            {
                // حالت حذف: hover قرمز
                AnimateBorderColor(ActionButton, _removeBgHover, _removeBorderHover);

            }
            else
            {
                // حالت افزودن: hover آبی
                AnimateBorderColor(ActionButton, _addBgHover, _addBorderHover);

            }
        }

        private void ActionButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetButtonColors();
        }

        // ══════════════════════════════════════════════════════
        //  Logic
        // ══════════════════════════════════════════════════════

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


                _hasImage = true;
            }
            catch
            {
                _hasImage = false;
            }
        }

        /// <summary>
        /// وضعیت کامل کنترل را به‌روزرسانی می‌کند.
        /// باید بعد از تغییر _hasImage فراخوانی شود.
        /// </summary>
        private void UpdateState(bool animate = true)
        {
            var duration = animate
                ? TimeSpan.FromSeconds(0.22)
                : TimeSpan.Zero;

            if (_hasImage)
            {
                // ── نمایش تصویر ──
                PlaceholderPanel.Visibility = Visibility.Collapsed;
                PreviewImage.Visibility = Visibility.Visible;

                // ── دکمه حذف ──

                SetSvg(ButtonIcon, RemoveIconSource);

                // رنگ پایه دکمه: قرمز
                SetBorderColors(ActionButton, _buttonBgDefault, _removeBorder);


                // border تصویر: سبز (نشان‌دهنده وجود تصویر)
                AnimateBorderStroke(ImageBorder, _imgBorderActive, duration);
            }
            else
            {
                // ── نمایش placeholder ──
                PreviewImage.Visibility = Visibility.Collapsed;
                PreviewImage.Source = null;
                PlaceholderPanel.Visibility = Visibility.Visible;

                // ── دکمه افزودن ──

                SetSvg(ButtonIcon, AddIconSource);

                // رنگ پایه دکمه: آبی
                SetBorderColors(ActionButton, _buttonBgDefault, _addBorder);

                // border تصویر: آبی
                AnimateBorderStroke(ImageBorder, _imgBorderDefault, duration);
            }
        }

        // ══════════════════════════════════════════════════════
        //  Helpers - SVG
        // ══════════════════════════════════════════════════════

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

        // ══════════════════════════════════════════════════════
        //  Helpers - Animation  (همه Brush جدید می‌سازند)
        // ══════════════════════════════════════════════════════

        /// <summary>رنگ Background و BorderBrush یک Border را بدون انیمیشن set می‌کند</summary>
        private void SetBorderColors(Border border, Color bg, Color stroke)
        {
            if (border == null) return;
            border.Background = new SolidColorBrush(bg);
            border.BorderBrush = new SolidColorBrush(stroke);
        }

        /// <summary>انیمیشن هم‌زمان Background و BorderBrush</summary>
        private void AnimateBorderColor(Border border, Color bgTo, Color strokeTo)
        {
            if (border == null) return;

            var dur = new Duration(TimeSpan.FromSeconds(0.15));

            // ── Background ──
            Color currentBg = Colors.Transparent;
            if (border.Background is SolidColorBrush bgBrush)
                currentBg = bgBrush.Color;

            var newBg = new SolidColorBrush(currentBg);   // غیر Frozen
            border.Background = newBg;
            newBg.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(bgTo, dur));

            // ── BorderBrush ──
            Color currentStroke = Colors.Transparent;
            if (border.BorderBrush is SolidColorBrush strokeBrush)
                currentStroke = strokeBrush.Color;

            var newStroke = new SolidColorBrush(currentStroke);  // غیر Frozen
            border.BorderBrush = newStroke;
            newStroke.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(strokeTo, dur));
        }

        /// <summary>انیمیشن فقط BorderBrush (برای ImageBorder)</summary>
        private void AnimateBorderStroke(Border border, Color to, TimeSpan? duration = null)
        {
            if (border == null) return;

            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.22));

            Color current = Colors.Transparent;
            if (border.BorderBrush is SolidColorBrush existing)
                current = existing.Color;

            var newBrush = new SolidColorBrush(current);  // غیر Frozen
            border.BorderBrush = newBrush;
            newBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(to, dur));
        }

        /// <summary>انیمیشن رنگ متن TextBlock</summary>
        private void AnimateTextColor(TextBlock tb, Color to, TimeSpan? duration = null)
        {
            if (tb == null) return;

            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.15));

            Color current = Colors.Black;
            if (tb.Foreground is SolidColorBrush existing)
                current = existing.Color;

            var newBrush = new SolidColorBrush(current);  // غیر Frozen
            tb.Foreground = newBrush;
            newBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                new ColorAnimation(to, dur));
        }

        /// <summary>بازگشت دکمه به رنگ پایه (بعد از hover)</summary>
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

        // ══════════════════════════════════════════════════════
        //  Dependency Property Callbacks
        // ══════════════════════════════════════════════════════

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
                ctrl.UpdateState(animate: true);

                // فقط اگر قبلاً تصویری داشته، رویداد حذف بفرست
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
