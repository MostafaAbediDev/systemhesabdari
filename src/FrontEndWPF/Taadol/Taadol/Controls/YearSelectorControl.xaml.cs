using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;

using System.Windows.Threading;

namespace Taadol.Controls
{
    /// <summary>
    /// سلکتور سال مالی در سایدبار.
    /// دوره‌های مالی رو از دیتابیس (IFinancialPeriodApplication) لود می‌کنه.
    /// سال مالی انتخاب‌شده رو از طریق Event به مصرف‌کننده گزارش می‌ده.
    /// </summary>
    public partial class YearSelectorControl : UserControl
    {
        private DispatcherTimer _collapseTimer;

        // ===== Event =====
        /// <summary>وقتی سال مالی عوض می‌شه صدا زده می‌شه (شناسه + عنوان)</summary>
        public event Action<long, string> SelectionChanged;
        private static readonly PersianCalendar _persianCalendar = new PersianCalendar();

        // ===== State =====
        private bool _isPopupOpen = false;
        private long _selectedPeriodId = 0;
        private string _selectedYearTitle = "";
        private List<FinancialPeriodViewModel> _periods = new();
        private bool _isLoaded = false;
        private static List<FinancialPeriodViewModel> _cachedPeriods = null;
        private static readonly SemaphoreSlim _cacheLock = new(1, 1);
        // ===== Constructor =====
        public YearSelectorControl()
        {
            InitializeComponent();

            // لود دوره‌های مالی از دیتابیس
            Loaded += OnLoaded;
        }

        // ======================================================
        //  Collapse / Expand (هنگام بسته/باز شدن سایدبار)
        // ======================================================

        /// <summary>
        /// هنگام بسته شدن سایدبار صدا زده می‌شه.
        /// متن و فلش رو FadeOut می‌کنه و بعد مخفی می‌کنه.
        /// خطوط بالا/پایین رو حذف و margin رو صفر می‌کنه تا آیکون وسط قرار بگیره.
        /// </summary>
        public void Collapse()
        {
            // ★ اگه تایمر قبلی هنوز در حال اجراست، متوقفش کن
            _collapseTimer?.Stop();

            FadeOut(YearText, 100);
            FadeOut(Chevron, 100);

            _collapseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
            _collapseTimer.Tick += (s, e) =>
            {
                _collapseTimer.Stop();
                YearText.Visibility = Visibility.Hidden;
                Chevron.Visibility = Visibility.Hidden;
            };
            _collapseTimer.Start();
        }

        /// <summary>
        /// هنگام باز شدن سایدبار صدا زده می‌شه.
        /// متن و فلش رو نشون می‌ده و FadeIn می‌کنه.
        /// خطوط و margin رو برمی‌گردونه.
        /// </summary>
        public void Expand()
        {
            // ★ مهم‌ترین خط: قبل از هرکاری، تایمر معلق Collapse رو متوقف کن
            // وگرنه بعداً دیر-اجرا می‌شه و دوباره Visibility رو Collapsed می‌کنه
            //_collapseTimer?.Stop();

            YearText.Visibility = Visibility.Visible;
            Chevron.Visibility = Visibility.Visible;

            FadeIn(YearText, 150);
            FadeIn(Chevron, 150);
        }
        private void FadeIn(UIElement el, int ms = 250)
        {
            el.Opacity = 0;
            el.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(ms),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });
        }

        private void FadeOut(UIElement el, int ms = 150)
        {
            el.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(ms),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            });
        }

        // ======================================================
        //  Load
        // ======================================================
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = true;

            await LoadPeriodsAsync();
        }

        /// <summary>
        /// لود دوره‌های مالی از دیتابیس به‌صورت async.
        /// فقط ۱۰ سال آخر رو لود می‌کنه.
        /// موقع لود، spinner نشون داده می‌شه.
        /// </summary>
        private async Task LoadPeriodsAsync()
        {
            // ★ اگه قبلاً کش شده، بدون هیچ اسپینر و تأخیری فوراً نمایش بده
            if (_cachedPeriods != null)
            {
                _periods = _cachedPeriods;
                ApplyLoadedPeriods();
                return;
            }

            ShowLoading(true);
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            try
            {
                await _cacheLock.WaitAsync();
                try
                {
                    // ★ دابل-چک: شاید در همین حین یه اینستنس دیگه کش کرده باشه
                    if (_cachedPeriods == null)
                    {
                        var items = await Task.Run(() =>
                        {
                            using var scope = App.ServiceProvider.CreateScope();
                            var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();
                            var allPeriods = app.GetFinancialPeriods() ?? new List<FinancialPeriodViewModel>();

                            return allPeriods
                                .OrderByDescending(p => p.Id)
                                .Take(10)
                                .ToList();
                        });

                        _cachedPeriods = items;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }

                _periods = _cachedPeriods;
                ApplyLoadedPeriods();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("YearSelector load failed: " + ex.Message);
                YearText.Text = "خطا در بارگذاری";
            }
            finally
            {
                ShowLoading(false);
            }
        }
        private void ApplyLoadedPeriods()
        {
            if (_periods.Count > 0 && _selectedPeriodId == 0)
            {
                _selectedPeriodId = _periods[0].Id;
                _selectedYearTitle = FormatPeriodTitle(_periods[0]);
                YearText.Text = _selectedYearTitle;
                SelectionChanged?.Invoke(_selectedPeriodId, _selectedYearTitle);
            }
            else if (_periods.Count == 0)
            {
                YearText.Text = "بدون دوره مالی";
            }
        }

        /// <summary>
        /// فرمت‌بندی عنوان دوره مالی:
        /// «شعبه [عنوان شعبه] | سال [سال]»
        /// مثال: «شعبه ۱۰۰ | سال ۱۴۰۵»
        /// </summary>
        private string FormatPeriodTitle(FinancialPeriodViewModel period)
        {
            var branch = string.IsNullOrWhiteSpace(period.BranchTitle) ? "-" : period.BranchTitle;
            branch = period.BranchTitle?.Replace("شعبه", "").Trim();

            int persianYear;
            if (DateTime.TryParse(period.StartDate, out var startDate))
            {
                persianYear = _persianCalendar.GetYear(startDate);
            }
            else
            {
                persianYear = 0;
            }

            // ★ استفاده از RLM (Right-to-Left Mark = \u200F) بعد از هر عدد
            // تا موتور Bidi مجبور بشه ترتیب رو دقیقاً همون‌طور که نوشتیم نگه ذاره
            const string RLM = "\u200F";
            return $"شعبه {branch}{RLM} | سال {persianYear}{RLM}";
        }
        /// <summary>نمایش/مخفی‌کردن spinner لود</summary>
        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            YearsList.Visibility = show ? Visibility.Hidden : Visibility.Visible;
        }

        // ======================================================
        //  Popup Open/Close
        // ======================================================
        private void Selector_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                OpenPopup();
        }

        private void OpenPopup()
        {
            BuildYearsList();
            YearsPopup.IsOpen = true;
            _isPopupOpen = true;
            RotateChevron(180);
        }
        private void ClosePopup()
        {
            YearsPopup.IsOpen = false;

        }

        private void RotateChevron(double angle)
        {
            ChevronRotation.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(angle, TimeSpan.FromMilliseconds(180))
                { EasingFunction = new CircleEase() });
        }
        private void YearsPopup_Closed(object sender, EventArgs e)
        {
            // ★ همگام‌سازی state با وضعیت واقعی پاپ‌آپ
            _isPopupOpen = false;
            RotateChevron(0);
        }
        // ======================================================
        //  Build Years List
        // ======================================================
        private void BuildYearsList()
        {
            YearsList.Children.Clear();

            for (int i = 0; i < _periods.Count; i++)
            {
                var period = _periods[i];
                bool isSelected = period.Id == _selectedPeriodId;

                // ★ Content رو به‌عنوان string قرار می‌دیم (نه TextBlock).
                // این کار باعث می‌شه Style دکمه (شامل FontWeight و Foreground) درست اعمال بشه.
                // Style دکمه در XAML طوری تنظیم شده که TextBlock داخلی رو راست‌چین کنه.
                var btn = new Button
                {
                    Content = FormatPeriodTitle(period),
                    Tag = period.Id,
                    // ★ FlowDirection اصلاً اینجا نباشه — از Border تو Template میاد
                    Style = (Style)Resources[isSelected ? "YearButtonSelectedStyle" : "YearButtonStyle"]
                };

                btn.Click += (s, e) =>
                {
                    var button = (Button)s;
                    var id = (long)button.Tag;
                    var title = button.Content.ToString();

                    _selectedPeriodId = id;
                    _selectedYearTitle = title;
                    YearText.Text = title;


                    SelectionChanged?.Invoke(id, title);
                    ClosePopup();
                };

                YearsList.Children.Add(btn);

                if (i < _periods.Count - 1)
                {
                    var divider = new Border { Style = (Style)Resources["YearDividerStyle"] };
                    YearsList.Children.Add(divider);
                }
            }
        }

        // ======================================================
        //  Helpers
        // ======================================================
        private string ToPersianDigits(int number)
        {
            return number.ToString()
                .Replace("0", "۰").Replace("1", "۱").Replace("2", "۲")
                .Replace("3", "۳").Replace("4", "۴").Replace("5", "۵")
                .Replace("6", "۶").Replace("7", "۷").Replace("8", "۸")
                .Replace("9", "۹");
        }
    }
}
