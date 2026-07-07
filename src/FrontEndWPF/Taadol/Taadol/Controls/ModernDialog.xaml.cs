using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Taadol.Controls
{
    /// <summary>
    /// دیالوگ مدرن با انیمیشن، آیکون رنگی و دکمه‌های زیبا.
    /// از متدهای استاتیک ShowConfirm / ShowInput استفاده کنید.
    /// </summary>
    public partial class ModernDialog : Window
    {
        public enum DialogType
        {
            Danger,    // قرمز - حذف
            Primary,   // آبی - افزودن
            Success,   // سبز - ویرایش / ثبت موفق
            Warning    // نارنجی - هشدار
        }

        public string InputText { get; private set; }
        public bool IsConfirmed { get; private set; }

        private bool _isClosing = false;

        /// <summary>پالت رنگ بر اساس نوع دیالوگ</summary>
        private class Palette
        {
            public string IconBg;        // رنگ پس‌زمینه دایره بیرونی
            public string IconInnerBg;   // رنگ پس‌زمینه دایره درونی
            public string TitleFg;       // رنگ عنوان
            public string ConfirmBg;     // رنگ دکمه تأیید
            public string ConfirmHover;  // رنگ hover دکمه تأیید
            public string IconPath;      // مسیر آیکون SVG
        }

        private static Palette GetPalette(DialogType type) => type switch
        {
            DialogType.Danger => new Palette
            {
                IconBg = "#FEE2E2",        // قرمز خیلی کم‌رنگ (حلقه بیرونی)
                IconInnerBg = "#FFFFFF",   // سفید (دایره داخلی برای وضوح آیکون)
                TitleFg = "#DC2626",
                ConfirmBg = "#E63946",
                ConfirmHover = "#C81E2D",
                IconPath = "/Assets/Icons/trash.svg"
            },
            DialogType.Primary => new Palette
            {
                IconBg = "#DBEAFE",        // آبی کم‌رنگ
                IconInnerBg = "#FFFFFF",
                TitleFg = "#2563EB",
                ConfirmBg = "#2563EB",
                ConfirmHover = "#1D4ED8",
                IconPath = "/Assets/Icons/Plus.svg"
            },
            DialogType.Success => new Palette
            {
                IconBg = "#DCFCE7",        // سبز کم‌رنگ
                IconInnerBg = "#FFFFFF",
                TitleFg = "#16A34A",
                ConfirmBg = "#16A34A",
                ConfirmHover = "#15803D",
                IconPath = "/Assets/Icons/edit.svg"
            },
            DialogType.Warning => new Palette
            {
                IconBg = "#FEF3C7",        // زرد کم‌رنگ
                IconInnerBg = "#FFFFFF",
                TitleFg = "#D97706",
                ConfirmBg = "#D97706",
                ConfirmHover = "#B45309",
                IconPath = "/Assets/Icons/Scale.svg"
            },
            _ => new Palette
            {
                IconBg = "#DBEAFE",
                IconInnerBg = "#FFFFFF",
                TitleFg = "#2563EB",
                ConfirmBg = "#2563EB",
                ConfirmHover = "#1D4ED8",
                IconPath = "/Assets/Icons/Plus.svg"
            }
        };

        public ModernDialog()
        {
            InitializeComponent();
            Loaded += ModernDialog_Loaded;
            KeyDown += ModernDialog_KeyDown;
        }

        // ==================== Static API ====================

        /// <summary>
        /// نمایش دیالوگ تأیید (بدون TextBox).
        /// </summary>
        public static bool ShowConfirm(
            string title,
            string message,
            DialogType type = DialogType.Warning,
            string confirmText = "تأیید",
            string cancelText = "انصراف",
            Window owner = null)
        {
            var dlg = new ModernDialog();
            dlg.Configure(title, message, type, confirmText, cancelText, showInput: false);
            if (owner != null) dlg.Owner = owner;
            dlg.ShowDialog();
            return dlg.IsConfirmed;
        }

        /// <summary>
        /// نمایش دیالوگ ورودی (با TextBox). مقدار وارد شده را برمی‌گرداند.
        /// اگر کاربر انصراف دهد، null برمی‌گردد.
        /// </summary>
        public static string ShowInput(
            string title,
            string message,
            string defaultValue = "",
            DialogType type = DialogType.Primary,
            string confirmText = "تأیید",
            string cancelText = "انصراف",
            Window owner = null)
        {
            var dlg = new ModernDialog();
            dlg.Configure(title, message, type, confirmText, cancelText, showInput: true, defaultValue);
            if (owner != null) dlg.Owner = owner;
            dlg.ShowDialog();
            return dlg.IsConfirmed ? dlg.InputText : null;
        }

        // ==================== Configuration ====================

        private void Configure(
            string title,
            string message,
            DialogType type,
            string confirmText,
            string cancelText,
            bool showInput,
            string defaultValue = "")
        {
            var p = GetPalette(type);

            DialogTitle.Text = title;
            DialogTitle.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(p.TitleFg);

            DialogMessage.Text = message;

            // آیکون و رنگ‌ها
            DialogIcon.Source = new System.Uri(p.IconPath, System.UriKind.Relative);
            IconCircle.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(p.IconBg);
            IconInner.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(p.IconInnerBg);

            // رنگ دکمه تأیید (از طریق Background دکمه، که در ControlTemplate به ConfirmBdInner bind شده)
            ConfirmButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(p.ConfirmBg);

            ConfirmText.Text = confirmText;
            CancelText.Text = cancelText;

            // نمایش / عدم نمایش TextBox
            if (showInput)
            {
                InputWrapper.Visibility = Visibility.Visible;
                InputBox.Text = defaultValue;
                InputBox.SelectAll();
                Loaded += (s, e) => InputBox.Focus();
            }
            else
            {
                InputWrapper.Visibility = Visibility.Collapsed;
            }
        }

        // ==================== Events ====================

        private void ModernDialog_Loaded(object sender, RoutedEventArgs e)
        {
            PlayOpenAnimation();
        }

        private void ModernDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Cancel_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                Confirm_Click(sender, e);
                e.Handled = true;
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Confirm_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Cancel_Click(sender, e);
                e.Handled = true;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (_isClosing) return;

            if (InputWrapper.Visibility == Visibility.Visible)
            {
                InputText = InputBox.Text;
                if (string.IsNullOrWhiteSpace(InputText))
                {
                    // کمی لرزش روی TextBox برای هشدار
                    var shake = new DoubleAnimationUsingKeyFrames { Duration = System.TimeSpan.FromMilliseconds(400) };
                    shake.KeyFrames.Add(new LinearDoubleKeyFrame(0, System.TimeSpan.FromMilliseconds(0)));
                    shake.KeyFrames.Add(new LinearDoubleKeyFrame(-6, System.TimeSpan.FromMilliseconds(80)));
                    shake.KeyFrames.Add(new LinearDoubleKeyFrame(6, System.TimeSpan.FromMilliseconds(160)));
                    shake.KeyFrames.Add(new LinearDoubleKeyFrame(-4, System.TimeSpan.FromMilliseconds(240)));
                    shake.KeyFrames.Add(new LinearDoubleKeyFrame(0, System.TimeSpan.FromMilliseconds(320)));

                    var rt = new TranslateTransform();
                    InputWrapper.RenderTransform = rt;
                    rt.BeginAnimation(TranslateTransform.XProperty, shake);

                    // فلش قرمز کردن Border
                    InputWrapper.BorderBrush = new SolidColorBrush(Color.FromRgb(0xE6, 0x39, 0x46));
                    return;
                }
            }

            IsConfirmed = true;
            PlayCloseAnimation(() =>
            {
                Close();
            });
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_isClosing) return;
            IsConfirmed = false;
            PlayCloseAnimation(() =>
            {
                Close();
            });
        }

        // ==================== Animations ====================

        private void PlayOpenAnimation()
        {
            // انیمیشن Scale از 0.85 به 1.0
            var scaleAnim = new DoubleAnimation
            {
                From = 0.85,
                To = 1.0,
                Duration = System.TimeSpan.FromMilliseconds(280),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.18 }
            };
            RootScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            RootScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);

            // انیمیشن Opacity از 0 به 1
            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = System.TimeSpan.FromMilliseconds(220),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            RootBorder.BeginAnimation(OpacityProperty, fade);

            // انیمیشن بالارفتن DropShadow
            var shadowAnim = new DoubleAnimation
            {
                From = 0,
                To = 0.18,
                Duration = System.TimeSpan.FromMilliseconds(300)
            };
            RootShadow.BeginAnimation(DropShadowEffect.OpacityProperty, shadowAnim);
        }

        private void PlayCloseAnimation(System.Action onComplete)
        {
            _isClosing = true;

            // یک timeout محافظتی: اگر انیمیشن به هر دلیلی کامل نشد، بعد از 500ms بسته شود
            var safetyTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(500)
            };
            safetyTimer.Tick += (s, e) =>
            {
                safetyTimer.Stop();
                onComplete?.Invoke();
            };
            safetyTimer.Start();

            var scaleAnim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.85,
                Duration = System.TimeSpan.FromMilliseconds(180),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            scaleAnim.Completed += (s, e) =>
            {
                safetyTimer.Stop();
                onComplete?.Invoke();
            };
            RootScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            RootScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);

            var fade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = System.TimeSpan.FromMilliseconds(180),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            RootBorder.BeginAnimation(OpacityProperty, fade);
        }
    }
}
