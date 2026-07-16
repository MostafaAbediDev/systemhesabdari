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

        private static readonly SolidColorBrush BlueBrush = new(Color.FromRgb(0x25, 0x63, 0xEB));
        private static readonly SolidColorBrush GrayBrush = new(Color.FromRgb(0x6B, 0x72, 0x80));
        private static readonly SolidColorBrush BorderGrayBrush = new(Color.FromRgb(0xE5, 0xE7, 0xEB));
        private static readonly SolidColorBrush WhiteBrush = Brushes.White;

        private const double AnimationDuration = 250;

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
            Loaded += PersonDetailPanel_Loaded;
        }

        private void PersonDetailPanel_Loaded(object sender, RoutedEventArgs e)
        {
            AnimatedContentBorder.MaxHeight = 1000;
        }

        public void LoadData(long id, string name, string personType,
            string category, string nationalId, string phone, string email,
            string city, string address, string balance, string balanceStatus,
            bool isActive)
        {
            _personId = id;
            PersonNameText.Text = name;
            PersonTypeText.Text = personType;
            CategoryText.Text = category;
            NationalIdText.Text = nationalId;
            PhoneText.Text = phone;
            EmailText.Text = email;
            CityText.Text = city;
            AddressText.Text = address;
            BalanceText.Text = balance;

            if (!string.IsNullOrEmpty(balanceStatus) && balanceStatus != "تسویه")
            {
                BalanceStatusBadge.Background = new SolidColorBrush(Color.FromRgb(0xFE, 0xF2, 0xF2));
                BalanceStatusText.Text = balanceStatus;
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xDC, 0x26, 0x26));
            }
            else
            {
                BalanceStatusBadge.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xFD, 0xF4));
                BalanceStatusText.Text = "تسویه";
                BalanceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
            }

            if (!string.IsNullOrEmpty(personType))
            {
                if (personType.Contains("مشتری"))
                {
                    PersonTypeBadge.Background = new SolidColorBrush(Color.FromRgb(0xEF, 0xF6, 0xFF));
                    PersonTypeText.Foreground = new SolidColorBrush(Color.FromRgb(0x25, 0x63, 0xEB));
                }
                else if (personType.Contains("تامین"))
                {
                    PersonTypeBadge.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xFD, 0xF4));
                    PersonTypeText.Foreground = new SolidColorBrush(Color.FromRgb(0x16, 0xA3, 0x4A));
                }
                else
                {
                    PersonTypeBadge.Background = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
                    PersonTypeText.Foreground = new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80));
                }
            }

            if (isActive)
            {
                ActiveStatusBadge.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xFD, 0xF4));
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

            var anim = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(AnimationDuration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            if (_isExpanded)
            {
                anim.To = 1000;
                CollapseIcon.Source = new Uri("/Assets/Icons/ArrowUP.svg", UriKind.Relative);
                RootBorder.CornerRadius = new CornerRadius(5, 5, 0, 0);
            }
            else
            {
                anim.To = 0;
                CollapseIcon.Source = new Uri("/Assets/Icons/arrowDowN.svg", UriKind.Relative);
                RootBorder.CornerRadius = new CornerRadius(5);
            }

            AnimatedContentBorder.BeginAnimation(MaxHeightProperty, anim);
        }

        private void EditIcon_Click(object sender, MouseButtonEventArgs e)
        {
            EditRequested?.Invoke(this, _personId);
        }

        private void DeleteIcon_Click(object sender, MouseButtonEventArgs e)
        {
            DeleteRequested?.Invoke(this, _personId);
        }

        private void CloseIcon_Click(object sender, MouseButtonEventArgs e)
        {
            CloseRequested?.Invoke(this, _personId);
        }

        private void SelectTab(Border activeTab, TextBlock activeText,
            Border inactiveTab1, TextBlock inactiveText1,
            Border inactiveTab2, TextBlock inactiveText2)
        {
            activeTab.Background = BlueBrush;
            activeTab.BorderBrush = BlueBrush;
            activeText.Foreground = WhiteBrush;

            inactiveTab1.Background = Brushes.Transparent;
            inactiveTab1.BorderBrush = BorderGrayBrush;
            inactiveText1.Foreground = GrayBrush;

            inactiveTab2.Background = Brushes.Transparent;
            inactiveTab2.BorderBrush = BorderGrayBrush;
            inactiveText2.Foreground = GrayBrush;
        }

        private void TabBasicInfo_Click(object sender, MouseButtonEventArgs e)
        {
            BasicInfoPanel.Visibility = Visibility.Visible;
            BankAccountsPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Collapsed;

            SelectTab(TabBasicInfo, TabBasicInfoText,
                      TabBankAccounts, TabBankAccountsText,
                      TabHistory, TabHistoryText);
        }

        private void TabBankAccounts_Click(object sender, MouseButtonEventArgs e)
        {
            BasicInfoPanel.Visibility = Visibility.Collapsed;
            BankAccountsPanel.Visibility = Visibility.Visible;
            HistoryPanel.Visibility = Visibility.Collapsed;

            SelectTab(TabBankAccounts, TabBankAccountsText,
                      TabBasicInfo, TabBasicInfoText,
                      TabHistory, TabHistoryText);
        }

        private void TabHistory_Click(object sender, MouseButtonEventArgs e)
        {
            BasicInfoPanel.Visibility = Visibility.Collapsed;
            BankAccountsPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Visible;

            SelectTab(TabHistory, TabHistoryText,
                      TabBasicInfo, TabBasicInfoText,
                      TabBankAccounts, TabBankAccountsText);
        }
    }
}
