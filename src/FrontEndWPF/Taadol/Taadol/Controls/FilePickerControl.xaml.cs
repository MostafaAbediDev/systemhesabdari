using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Controls
{
    public partial class FilePickerControl : UserControl
    {
        private static readonly Color _addBorder = (Color)ColorConverter.ConvertFromString("#2667FF");
        private static readonly Color _addBorderHover = (Color)ColorConverter.ConvertFromString("#1A4FCC");
        private static readonly Color _addBgHover = (Color)ColorConverter.ConvertFromString("#EEF3FF");
        private static readonly Color _removeBorder = (Color)ColorConverter.ConvertFromString("#FF4D4F");
        private static readonly Color _removeBorderHover = (Color)ColorConverter.ConvertFromString("#CC1A1C");
        private static readonly Color _removeBgHover = (Color)ColorConverter.ConvertFromString("#FFEBEB");
        private static readonly Color _imgBorderDefault = (Color)ColorConverter.ConvertFromString("#BED1FF");
        private static readonly Color _imgBorderActive = (Color)ColorConverter.ConvertFromString("#A5D6A7");
        private static readonly Color _buttonBgDefault = Colors.White;

        private string _filePath = null;

        public FilePickerControl()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateState(animate: false);
        }

        public event EventHandler FileSelected;
        public event EventHandler FileCleared;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                UpdateState(animate: true);
            }
        }

        public string FileExtension
        {
            get
            {
                if (string.IsNullOrEmpty(_filePath)) return "";
                return Path.GetExtension(_filePath).ToLower();
            }
        }

        private void ActionButton_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFilePicker();
        }

        private void ClearButton_Click(object sender, MouseButtonEventArgs e)
        {
            FilePath = null;
            FileCountText.Visibility = Visibility.Collapsed;
            FileCleared?.Invoke(this, EventArgs.Empty);
        }

        private void ActionButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateBorderColor(ActionButton, _addBgHover, _addBorderHover);
        }

        private void ActionButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SetBorderColors(ActionButton, _buttonBgDefault, _addBorder);
        }

        private void ClearButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateBorderColor(ClearButton, _removeBgHover, _removeBorderHover);
        }

        private void ClearButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SetBorderColors(ClearButton, _buttonBgDefault, _removeBorder);
        }

        private void OpenFilePicker()
        {
            var dialog = new OpenFileDialog
            {
                Title = "انتخاب فایل",
                Filter = "فایل‌های مجاز|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.pdf;*.docx;*.doc;*.xlsx;*.xls;*.zip;*.rar;*.exe;*.txt|همه فایل‌ها|*.*",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
                FileCountText.Text = Path.GetFileName(dialog.FileName);
                FileCountText.Visibility = Visibility.Visible;
                FileSelected?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateState(bool animate = true)
        {
            var duration = animate ? TimeSpan.FromSeconds(0.22) : TimeSpan.Zero;

            if (!string.IsNullOrEmpty(_filePath))
            {
                PlaceholderLabel.Text = "";
                FileCountText.Visibility = Visibility.Visible;
                ClearButton.Visibility = Visibility.Visible;
                SetSvg(ButtonIcon, "/Assets/Icons/add-square.svg");
                SetBorderColors(ActionButton, _buttonBgDefault, _addBorder);
                AnimateBorderStroke(FileBorder, _imgBorderActive, duration);
            }
            else
            {
                PlaceholderLabel.Text = "انتخاب فایل";
                FileCountText.Visibility = Visibility.Collapsed;
                ClearButton.Visibility = Visibility.Collapsed;
                SetSvg(ButtonIcon, "/Assets/Icons/add-square.svg");
                SetBorderColors(ActionButton, _buttonBgDefault, _addBorder);
                AnimateBorderStroke(FileBorder, _imgBorderDefault, duration);
            }
        }

        private void SetSvg(SharpVectors.Converters.SvgViewbox box, string source)
        {
            if (box == null || string.IsNullOrEmpty(source)) return;
            try { box.Source = new Uri(source, UriKind.RelativeOrAbsolute); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("FilePicker SetSvg failed: " + ex.Message); }
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
            var newBg = new SolidColorBrush(Colors.Transparent);
            border.Background = newBg;
            newBg.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(bgTo, dur));
            var newStroke = new SolidColorBrush(Colors.Transparent);
            border.BorderBrush = newStroke;
            newStroke.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(strokeTo, dur));
        }

        private void AnimateBorderStroke(Border border, Color to, TimeSpan? duration = null)
        {
            if (border == null) return;
            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.22));
            Color current = Colors.Transparent;
            if (border.BorderBrush is SolidColorBrush existing)
                current = existing.Color;
            var newBrush = new SolidColorBrush(current);
            border.BorderBrush = newBrush;
            newBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(to, dur));
            AnimateDashedBorderStroke(to, duration);
        }

        private void AnimateDashedBorderStroke(Color to, TimeSpan? duration = null)
        {
            if (DashedBorder == null) return;
            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.22));
            Color current = Colors.Transparent;
            if (DashedBorder.Stroke is SolidColorBrush existing)
                current = existing.Color;
            var newBrush = new SolidColorBrush(current);
            DashedBorder.Stroke = newBrush;
            newBrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(to, dur));
        }
    }
}
