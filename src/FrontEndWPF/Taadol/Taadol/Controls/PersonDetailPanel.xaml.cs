using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Controls
{
    public partial class PersonDetailPanel : UserControl
    {
        private bool _isExpanded = true;
        private long _personId;

        private static readonly SolidColorBrush WhiteBrush = Brushes.White;

        private const double AnimationDuration = 250;
        private const double OpenDuration = 380;
        private const double CloseDuration = 300;

        private static readonly SolidColorBrush BlueBrush = new(Color.FromRgb(0x25, 0x63, 0xEB));
        private static readonly SolidColorBrush GrayBrush = new(Color.FromRgb(0x6B, 0x72, 0x80));
        private static readonly SolidColorBrush ActiveGrayBrush = new(Color.FromRgb(0x4B, 0x52, 0x63)); // کنتراست بیشتر

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
            Loaded += (_, _) => AnimatedContentBorder.MaxHeight = 1000;
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
            string city, string address, string balance, string balanceStatus,
            bool isActive)
        {
            _personId = id;
            PersonNameText.Text = name;
            CategoryText.Text = category;
            NationalIdText.Text = nationalId;
            PhoneText.Text = phone;
            EmailText.Text = email;
            CityText.Text = city;
            AddressText.Text = address;
            BalanceText.Text = balance;

            if (!string.IsNullOrEmpty(balanceStatus) && balanceStatus != "تسویه")
            {
                BalanceStatusText.Text = balanceStatus;
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
                BalanceText.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
            }
            else
            {
                BalanceStatusText.Text = "تسویه";
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
                BalanceText.Foreground = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
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

        private void CollapseBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isExpanded = !_isExpanded;

            double duration = _isExpanded ? OpenDuration : CloseDuration;
            var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };

            // ── Height animation ──
            var heightAnim = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(duration),
                EasingFunction = ease
            };

            // ── Opacity animation ──
            var opacityAnim = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(duration * 0.6),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // ── Rotate animation (arrow icon) ──
            var rotateAnim = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(duration * 0.8),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut, Amplitude = 0.3 }
            };

            if (_isExpanded)
            {
                heightAnim.To = 800;
                opacityAnim.To = 1;
                rotateAnim.To = 0;
            }
            else
            {
                heightAnim.To = 0;
                opacityAnim.To = 0;
                rotateAnim.To = 180;
            }

            AnimatedContentBorder.BeginAnimation(MaxHeightProperty, heightAnim);
            AnimatedContentBorder.BeginAnimation(OpacityProperty, opacityAnim);
            CollapseIconRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
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
