using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Taadol.Views;

namespace Taadol.Controls
{
    /// <summary>
    /// جدول حساب‌های بانکی مشترک بین NewPersonView و EditPersonView.
    /// با عرض کم، به‌جای Clip شدن، اسکرول افقی نشان می‌دهد.
    /// </summary>
    public partial class BankAccountsTableControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(BankAccountsTableControl),
                new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>اگر true باشد، حذف ردیف نیاز به تأیید کاربر دارد (رفتار فرم ویرایش).</summary>
        public static readonly DependencyProperty RemoveConfirmationProperty =
            DependencyProperty.Register(nameof(RemoveConfirmation), typeof(bool), typeof(BankAccountsTableControl),
                new PropertyMetadata(false));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public bool RemoveConfirmation
        {
            get => (bool)GetValue(RemoveConfirmationProperty);
            set => SetValue(RemoveConfirmationProperty, value);
        }

        public event EventHandler<BankAccountRow> EditRequested;
        public event EventHandler<BankAccountRow> RemoveRequested;

        public BankAccountsTableControl()
        {
            InitializeComponent();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (BankAccountsTableControl)d;
            if (e.OldValue is INotifyCollectionChanged oldColl)
                oldColl.CollectionChanged -= ctrl.OnCollectionChanged;
            if (e.NewValue is INotifyCollectionChanged newColl)
                newColl.CollectionChanged += ctrl.OnCollectionChanged;
            ctrl.UpdateVisibility();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => UpdateVisibility();

        /// <summary>جدول وقتی خالی است Collapsed می‌شود (مثل Visibility قبلی با CountToVisibilityConverter).</summary>
        private void UpdateVisibility()
        {
            int count = 0;
            if (ItemsSource is ICollection coll)
            {
                count = coll.Count;
            }
            else if (ItemsSource != null)
            {
                var enumerator = ItemsSource.GetEnumerator();
                if (enumerator.MoveNext()) count = 1;
                (enumerator as IDisposable)?.Dispose();
            }
            Visibility = count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EditBankAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BankAccountRow row)
                EditRequested?.Invoke(this, row);
        }

        private void RemoveBankAccountFromTable_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BankAccountRow row)
            {
                if (RemoveConfirmation)
                {
                    var result = MessageBox.Show("آیا از حذف این حساب بانکی مطمئن هستید؟", "تایید حذف",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;
                }
                RemoveRequested?.Invoke(this, row);
            }
        }

        private void TableContainerBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var border = (Border)sender;
            double radius = border.CornerRadius.TopLeft; // چون همه گوشه‌ها 4 هستند
            border.Clip = new RectangleGeometry(
                new Rect(0, 0, border.ActualWidth, border.ActualHeight),
                radius,
                radius);
        }
    }
}
