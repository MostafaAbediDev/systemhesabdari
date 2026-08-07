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
            NextPageRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            PreviousPageRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null && int.TryParse(btn.Tag.ToString(), out var page))
                PageRequested?.Invoke(this, page);
        }

        private void PageSizeSelector_SelectionChanged(object sender, int newSize)
        {
            PageSizeRequested?.Invoke(this, newSize);
        }
    }
}