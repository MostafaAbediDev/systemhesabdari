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

    public partial class YearSelectorControl : UserControl
    {
        private DispatcherTimer _collapseTimer;

        public event Action<long, string> SelectionChanged;
        private static readonly PersianCalendar _persianCalendar = new PersianCalendar();

        private bool _isPopupOpen = false;
        private long _selectedPeriodId = 0;
        private string _selectedYearTitle = "";
        private List<FinancialPeriodViewModel> _periods = new();
        private bool _isLoaded = false;
        private CancellationTokenSource _loadCts = new();
        private int _loadVersion;
        private static List<FinancialPeriodViewModel> _cachedPeriods = null;
        private static readonly SemaphoreSlim _cacheLock = new(1, 1);

        public YearSelectorControl()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public void Collapse()
        {

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

        public void Expand()
        {

            _collapseTimer?.Stop();

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

        public static void InvalidateCache()
        {
            _cachedPeriods = null;
        }

        public async Task RefreshAsync()
        {
            _isLoaded = true;
            _cachedPeriods = null;
            CancelPendingLoad();
            await LoadPeriodsAsync();
        }

        private void CancelPendingLoad()
        {
            Interlocked.Increment(ref _loadVersion);
            var cts = Interlocked.Exchange(ref _loadCts, new CancellationTokenSource());
            try { cts.Cancel(); } catch (ObjectDisposedException) { }
            cts.Dispose();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CancelPendingLoad();
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = true;

            await LoadPeriodsAsync();
        }

        private async Task LoadPeriodsAsync()
        {
            var version = Volatile.Read(ref _loadVersion);
            var token = _loadCts.Token;

            if (_cachedPeriods != null)
            {
                if (token.IsCancellationRequested || version != Volatile.Read(ref _loadVersion)) return;
                _periods = _cachedPeriods;
                ApplyLoadedPeriods();
                return;
            }

            ShowLoading(true);
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            try
            {
                await _cacheLock.WaitAsync(token);
                try
                {

                    if (_cachedPeriods == null)
                    {
                        var items = await Task.Run(() =>
                        {
                            token.ThrowIfCancellationRequested();
                            using var scope = App.ServiceProvider.CreateScope();
                            var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();
                            var allPeriods = app.GetFinancialPeriods() ?? new List<FinancialPeriodViewModel>();

                            return allPeriods
                                .OrderByDescending(p => p.Id)
                                .Take(10)
                                .ToList();
                        }, token);

                        token.ThrowIfCancellationRequested();
                        _cachedPeriods = items;
                    }
                }
                finally
                {
                    _cacheLock.Release();
                }

                if (token.IsCancellationRequested || version != Volatile.Read(ref _loadVersion)) return;
                _periods = _cachedPeriods;
                ApplyLoadedPeriods();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                if (version != Volatile.Read(ref _loadVersion)) return;
                System.Diagnostics.Debug.WriteLine("YearSelector load failed: " + ex.Message);
                YearText.Text = "خطا در بارگذاری";
            }
            finally
            {
                if (version == Volatile.Read(ref _loadVersion) && !token.IsCancellationRequested)
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

            const string RLM = "\u200F";
            return $"شعبه {branch}{RLM} | سال {persianYear}{RLM}";
        }

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Hidden;
            YearsList.Visibility = show ? Visibility.Hidden : Visibility.Visible;
        }

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

            _isPopupOpen = false;
            RotateChevron(0);
        }

        private void BuildYearsList()
        {
            YearsList.Children.Clear();

            for (int i = 0; i < _periods.Count; i++)
            {
                var period = _periods[i];
                bool isSelected = period.Id == _selectedPeriodId;

                var btn = new Button
                {
                    Content = FormatPeriodTitle(period),
                    Tag = period.Id,

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
