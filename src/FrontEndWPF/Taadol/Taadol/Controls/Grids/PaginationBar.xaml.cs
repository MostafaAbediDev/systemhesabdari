using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Taadol.Controls
{
    /// <summary>
    /// نوار صفحه‌بندی مشترک: دکمه قبلی/بعدی، شماره صفحات، انتخاب تعداد صفحه و متن شمارنده.
    /// فرم‌ها از طریق شبکه رویدادها مقادیر را دریافت/ایجاد می‌کنند.
    /// </summary>
    public partial class PaginationBar : UserControl
    {
        public PaginationBar()
        {
            InitializeComponent();
        }

        // ────────────────────── Dependency Properties ──────────────────────

        public static readonly DependencyProperty PagesSourceProperty =
            DependencyProperty.Register(nameof(PagesSource), typeof(IEnumerable), typeof(PaginationBar),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (PaginationBar)d;
                    if (control.PageButtonsItemsControl != null)
                        control.PageButtonsItemsControl.ItemsSource = control.PagesSource;
                }));

        public IEnumerable PagesSource
        {
            get => (IEnumerable)GetValue(PagesSourceProperty);
            set => SetValue(PagesSourceProperty, value);
        }

        public static readonly DependencyProperty PageInfoContentProperty =
            DependencyProperty.Register(nameof(PageInfoContent), typeof(string), typeof(PaginationBar),
                new PropertyMetadata(null, (d, _) =>
                {
                    var control = (PaginationBar)d;
                    if (control.PageInfoText != null)
                        control.PageInfoText.Text = control.PageInfoContent;
                }));

        public string PageInfoContent
        {
            get => (string)GetValue(PageInfoContentProperty);
            set => SetValue(PageInfoContentProperty, value);
        }

        // ────────────────────── Events ──────────────────────

        public event EventHandler NextPageRequested;
        public event EventHandler PreviousPageRequested;
        public event EventHandler<int> PageRequested;
        public event EventHandler<int> PageSizeRequested;

        // ────────────────────── Handlers ──────────────────────

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (!TryBeginInteraction()) return;
            NextPageRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (!TryBeginInteraction()) return;
            PreviousPageRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null ||
                !int.TryParse(btn.Tag.ToString(), out var page) || page <= 0)
                return;

            if (!TryBeginInteraction()) return;
            PageRequested?.Invoke(this, page);
        }

        private int _lastRequestedPageSize = -1;
        private bool _interactionPending;
        private int _interactionVersion;

        public static readonly DependencyProperty IsProcessingProperty =
            DependencyProperty.Register(nameof(IsProcessing), typeof(bool), typeof(PaginationBar),
                new PropertyMetadata(false, (d, _) => ((PaginationBar)d).UpdateEnabledState()));

        public bool IsProcessing
        {
            get => (bool)GetValue(IsProcessingProperty);
            set => SetValue(IsProcessingProperty, value);
        }

        private bool TryBeginInteraction()
        {
            if (_interactionPending || IsProcessing)
                return false;

            _interactionPending = true;
            UpdateEnabledState();
            var version = ++_interactionVersion;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (version == _interactionVersion)
                {
                    _interactionPending = false;
                    UpdateEnabledState();
                }
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
            return true;
        }

        private void UpdateEnabledState()
        {
            IsEnabled = !IsProcessing && !_interactionPending;
        }

        private void PageSizeSelector_SelectionChanged(object sender, int newSize)
        {
            // گارد دوم در لایه‌ی واسط: حتی اگر کنترل داخلی به‌دلیل رویداد ورودی
            // چند بار پیام بدهد، فرم فقط یک بار برای همان مقدار Refresh می‌شود.
            if (_lastRequestedPageSize == newSize)
                return;

            if (!TryBeginInteraction())
                return;

            _lastRequestedPageSize = newSize;
            PageSizeRequested?.Invoke(this, newSize);
        }
    }
}