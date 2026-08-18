using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Taadol.Models;

namespace Taadol.Controls
{
    public partial class PersonDetailPanel : UserControl
    {
        private bool _isExpanded;
        private long _personId;

        private const double OpenDuration = 380;
        private const double CloseDuration = 300;

        private static readonly SolidColorBrush BlueBrush = new(Color.FromRgb(0x25, 0x63, 0xEB));
        private static readonly SolidColorBrush ActiveGrayBrush = new(Color.FromRgb(0x4B, 0x52, 0x63));

        // پیش‌فرض: پنل به‌صورت بسته لود می‌شه (شبیه تصویر). اگه لازمه بازش بمونه، این رو true بذار.
        public bool StartExpanded { get; set; } = true;

        public long PersonId
        {
            get => _personId;
            set => _personId = value;
        }

        public event EventHandler<long> CloseRequested;
        public event EventHandler<long> EditRequested;
        public event EventHandler<long> DeleteRequested;

        public PersonDetailPanel()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                _isExpanded = StartExpanded;
                ApplyExpandState(_isExpanded, animate: false);
            };
        }

        private void RootBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ContentScroll == null) return;

            if (e.Delta > 0)
                ContentScroll.LineUp();
            else
                ContentScroll.LineDown();

            e.Handled = true;
        }

        public void LoadData(long id, string name, string personType,
            string category, string nationalId, string phone, string email,
            string city, string address, string postalCode, string balance,
            string balanceStatus, bool isActive)
        {
            _personId = id;
            PersonNameText.Text = name;
            CategoryText.Text = category;
            NationalIdText.Text = nationalId;
            PhoneText.Text = phone;
            EmailText.Text = email;
            CityText.Text = city;
            AddressText.Text = address;
            PostalCodeText.Text = string.IsNullOrWhiteSpace(postalCode)
                ? ""
                : "کد پستی: " + postalCode;
            BalanceText.Text = balance + " ریال";

            if (balanceStatus == "بدهکار")
            {
                BalanceStatusText.Text = balanceStatus;
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x22, 0xC5, 0x5E));
                BalanceText.Foreground = new SolidColorBrush(Color.FromRgb(0x22, 0xC5, 0x5E));
            }
            else if (balanceStatus == "بستانکار")
            {
                BalanceStatusText.Text = balanceStatus;
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
                BalanceText.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
            }
            else
            {
                BalanceStatusText.Text = "بی حساب";
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B));
                BalanceText.Foreground = new SolidColorBrush(Color.FromRgb(0x64, 0x74, 0x8B));
            }

            if (isActive)
            {
                ActiveStatusBadge.Background = new SolidColorBrush(Color.FromRgb(0xEC, 0xFD, 0xF5));
                ActiveStatusText.Text = "فعال";
                ActiveStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
            }
            else
            {
                ActiveStatusBadge.Background = new SolidColorBrush(Color.FromRgb(0xFE, 0xF2, 0xF2));
                ActiveStatusText.Text = "غیرفعال";
                ActiveStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
            }
        }

        public void LoadBankAccounts(List<BankAccountItem> accounts)
        {
            if (accounts == null || accounts.Count == 0)
            {
                BankAccountContentBorder.Visibility = Visibility.Collapsed;
                return;
            }

            BankAccountContentBorder.Visibility = Visibility.Visible;

            var defaultAccount = accounts.FirstOrDefault(a => a.IsDefault) ?? accounts[0];

            BankNameText.Text = defaultAccount.BankName ?? "—";
            BranchNameText.Text = defaultAccount.BranchName ?? "—";
            CardNumberText.Text = defaultAccount.CardNumber ?? "—";
            ShebaNumberText.Text = defaultAccount.ShebaNumber ?? "—";

            // «حساب دیگر دارد» فقط وقتی مجموع حساب‌ها بیشتر از ۱ باشد
            OtherAccountText.Text = accounts.Count > 1 ? "دارد" : "ندارد";
        }

        public void LoadTransactions(List<TransactionItem> transactions)
        {
            if (TransactionsItems != null)
                TransactionsItems.ItemsSource = transactions;
        }

        private void CollapseBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isExpanded = !_isExpanded;
            ApplyExpandState(_isExpanded, animate: true);
        }

        /// <summary>
        /// حالت باز/بسته رو اعمال می‌کنه. اگه animate=false باشه (مثلاً موقع لود اولیه)
        /// مقادیر مستقیم و بدون انیمیشن ست می‌شن تا هیچ پرش/فلیکری دیده نشه
        /// و پنل دقیقاً همون شکلِ نهاییِ بسته/باز رو از همون لحظه اول داشته باشه.
        /// </summary>
        private void ApplyExpandState(bool expanded, bool animate)
        {
            double heightTarget = expanded ? 800 : 0;
            double opacityTarget = expanded ? 1 : 0;
            double rotateTarget = expanded ? 0 : 180;

            var balThickness = expanded ? new Thickness(0, 1, 0, 1) : new Thickness(0);
            var balPadding = expanded ? new Thickness(0, 14, 0, 14) : new Thickness(0, 8, 0, 8);
            var balMargin = expanded ? new Thickness(0, 12, 0, 0) : new Thickness(0, 6, 0, 0);

            if (!animate)
            {
                AnimatedContentBorder.MaxHeight = heightTarget;
                AnimatedContentBorder.Opacity = opacityTarget;
                CollapseIconRotate.Angle = rotateTarget;
                BalanceSectionBorder.BorderThickness = balThickness;
                BalanceSectionBorder.Padding = balPadding;
                BalanceSectionBorder.Margin = balMargin;
                return;
            }

            double duration = expanded ? OpenDuration : CloseDuration;
            var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };

            var heightAnim = new DoubleAnimation
            {
                To = heightTarget,
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = ease
            };

            var opacityAnim = new DoubleAnimation
            {
                To = opacityTarget,
                Duration = TimeSpan.FromMilliseconds(duration * 0.6),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var rotateAnim = new DoubleAnimation
            {
                To = rotateTarget,
                Duration = TimeSpan.FromMilliseconds(duration * 0.8),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut, Amplitude = 0.3 }
            };

            var balTime = TimeSpan.FromMilliseconds(duration);
            var thicknessAnim = new ThicknessAnimation(balThickness, balTime) { EasingFunction = ease };
            var padAnim = new ThicknessAnimation(balPadding, balTime) { EasingFunction = ease };
            var marAnim = new ThicknessAnimation(balMargin, balTime) { EasingFunction = ease };

            AnimatedContentBorder.BeginAnimation(MaxHeightProperty, heightAnim);
            AnimatedContentBorder.BeginAnimation(OpacityProperty, opacityAnim);
            CollapseIconRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
            BalanceSectionBorder.BeginAnimation(Border.BorderThicknessProperty, thicknessAnim);
            BalanceSectionBorder.BeginAnimation(Border.PaddingProperty, padAnim);
            BalanceSectionBorder.BeginAnimation(FrameworkElement.MarginProperty, marAnim);
        }

        private void EditIcon_Click(object sender, MouseButtonEventArgs e)
            => EditRequested?.Invoke(this, _personId);

        private void DeleteIcon_Click(object sender, MouseButtonEventArgs e)
            => DeleteRequested?.Invoke(this, _personId);

        private void CloseIcon_Click(object sender, MouseButtonEventArgs e)
            => CloseRequested?.Invoke(this, _personId);

        private void SelectTab(Border activeTab, TextBlock activeText,
             Border inactiveTab1, TextBlock inactiveText1,
             Border inactiveTab2, TextBlock inactiveText2)
        {
            activeTab.BorderBrush = BlueBrush;
            activeText.Foreground = BlueBrush;
            activeText.FontWeight = FontWeights.Bold;

            inactiveTab1.BorderBrush = Brushes.Transparent;
            inactiveText1.Foreground = ActiveGrayBrush;
            inactiveText1.FontWeight = FontWeights.Medium;

            inactiveTab2.BorderBrush = Brushes.Transparent;
            inactiveText2.Foreground = ActiveGrayBrush;
            inactiveText2.FontWeight = FontWeights.Medium;
        }

        private void TabBasicInfo_Checked(object sender, RoutedEventArgs e)
        {
            if (BasicInfoPanel == null) return;
            BasicInfoPanel.Visibility = Visibility.Visible;
            BankAccountsPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Collapsed;
        }

        private void TabBankAccounts_Checked(object sender, RoutedEventArgs e)
        {
            if (BasicInfoPanel == null) return;
            BasicInfoPanel.Visibility = Visibility.Collapsed;
            BankAccountsPanel.Visibility = Visibility.Visible;
            HistoryPanel.Visibility = Visibility.Collapsed;
        }

        private void TabHistory_Checked(object sender, RoutedEventArgs e)
        {
            if (BasicInfoPanel == null) return;
            BasicInfoPanel.Visibility = Visibility.Collapsed;
            BankAccountsPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Visible;
        }
    }
}