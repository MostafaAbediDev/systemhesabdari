using MaterialDesignThemes.Wpf.Internal;
using SharpVectors.Converters;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Taadol.Controls
{
    public partial class SidebarControl : UserControl
    {
        private string _activeSubMenuTag = null;

        private Button _activeSubMenuButton = null;
        private bool _isSubMenuAnimating = false;
        private bool _isSidebarOpen = true;
        private bool _isAnimating = false;
        private const double SIDEBAR_OPEN = 240;
        private const double SIDEBAR_CLOSED = 62;
        private Dictionary<string, StackPanel> _subMenus;
        private Button _activeMenuButton = null;

        public event Action<double> SidebarWidthChanged;
        public event Action<string> SubMenuClicked;

        public SidebarControl()
        {
            InitializeComponent();
            BuildYearList();
            YearPopup.Closed += (s, e) =>
            {
                _isYearPopupOpen = false;
                RotateYearChevron(0);
            };
            YearChevron.Visibility = Visibility.Visible;

            var rotation = FindArrowRotation(BtnToggleSidebar);
            if (rotation != null)
                rotation.Angle = 180;

            _subMenus = new Dictionary<string, StackPanel>
{
    { "dashboard", SubDashboard },
    { "products", SubProducts },
    { "persons", SubPersons },
    { "businessinfo", SubBusinessInfo },
    { "warehouse", SubWarehouse },
    { "bank", SubBank },
    { "sales", SubSales },
    { "reports", SubReports }
};

        }

        private void SubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var clickedBtn = sender as Button;
            if (clickedBtn == null) return;


            if (_activeSubMenuItem != null)
                SetSubMenuInactive(_activeSubMenuItem);

            _activeSubMenuItem = clickedBtn;
            SetSubMenuActive(clickedBtn);
        }

        private Button _activeSubMenuItem = null;

        private void SetActiveMenu(Button button, string menuTag)
        {
            if (_activeMenuButton != null)
            {
                SetMenuInactive(_activeMenuButton);
            }

            _activeMenuButton = button;
            SetMenuActive(button, menuTag);
        }

        private void SetMenuActive(Button button, string menuTag)
        {
            var border = button.Template.FindName("bd", button) as Border;
            if (border != null)
            {
                border.SetResourceReference(Border.BackgroundProperty, "ActiveMenuBrush");


                if (_subMenus.ContainsKey(menuTag))
                {
                    var subMenu = _subMenus[menuTag];

                    if (subMenu.Visibility == Visibility.Visible)
                    {
                        border.CornerRadius = new CornerRadius(8, 8, 0, 0);
                    }
                    else
                    {
                        border.CornerRadius = new CornerRadius(8);
                    }
                }
                else
                {
                    border.CornerRadius = new CornerRadius(8);
                }
            }


            string iconName = GetIconName(menuTag, false);
            var icon = FindChild<SvgViewbox>(button, null);
            if (icon != null && !string.IsNullOrEmpty(iconName))
            {
                icon.Source = new Uri($"/Assets/Icons/{iconName}.svg", UriKind.Relative);
            }
        }

        private void SetMenuInactive(Button button)
        {
            var border = button.Template.FindName("bd", button) as Border;
            if (border != null)
            {
                border.ClearValue(Border.BackgroundProperty);
                border.ClearValue(Border.BackgroundProperty);
                border.CornerRadius = new CornerRadius(8);
            }

            string menuTag = button.Tag as string ?? GetMenuTagFromButton(button);
            string iconName = GetIconName(menuTag, true);
            var icon = FindChild<SvgViewbox>(button, null);
            if (icon != null && !string.IsNullOrEmpty(iconName))
            {
                icon.Source = new Uri($"/Assets/Icons/{iconName}.svg", UriKind.Relative);
            }
        }
        private void UpdateMenuCornerRadius(Button button, string menuTag)
        {
            if (!_subMenus.ContainsKey(menuTag)) return;

            var border = button.Template.FindName("bd", button) as Border;
            if (border == null)
                border = FindChild<Border>(button, "bd");

            if (border == null) return;

            var subMenu = _subMenus[menuTag];
            CornerRadius targetRadius;

            if (subMenu.Visibility == Visibility.Visible)
            {
                targetRadius = new CornerRadius(8, 8, 0, 0);
            }
            else
            {
                targetRadius = new CornerRadius(8);
            }

            AnimateCornerRadius(border, targetRadius, 20);
        }
        private string GetMenuTagFromButton(Button button)
        {
            if (button == BtnDashboard) return "dashboard";
            if (button == BtnProducts) return "products";
            if (button == BtnPersons) return "persons";
            if (button == BtnBusinessInfo) return "businessinfo";
            if (button == BtnWarehouse) return "warehouse";
            if (button == BtnBank) return "bank";
            if (button == BtnSales) return "sales";
            if (button == BtnReports) return "reports";
            return "";
        }

        private string GetIconName(string menuTag, bool inactive)
        {
            string suffix = inactive ? "_u" : "";
            switch (menuTag)
            {
                case "dashboard": return "briefcase" + suffix;
                case "products": return "chart" + suffix;
                case "persons": return "profile" + suffix;
                case "businessinfo": return inactive ? "NewBranchView_off" : "NewBranchView";
                case "warehouse": return "buildings" + suffix;
                case "bank": return "dollar-circle" + suffix;
                case "sales": return "share" + suffix;
                case "reports": return "rep" + suffix;
                default: return "";
            }
        }

        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnimating) return;
            _isSidebarOpen = !_isSidebarOpen;
            if (_isSidebarOpen) AnimateOpen();
            else AnimateClose();
        }

        private void AnimateOpen()
        {
            _isAnimating = true;
            var rotation = FindArrowRotation(BtnToggleSidebar);
            rotation?.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(180, TimeSpan.FromMilliseconds(180))
                { EasingFunction = new CircleEase() });

            var arrow = FindChild<SvgViewbox>(BtnToggleSidebar, "ToggleArrowIcon");
            if (arrow != null) arrow.Source = new Uri("/Assets/Icons/chevron-left.svg", UriKind.Relative);

            YearChevron.Visibility = Visibility.Visible;
            SetAllTextsVisibility(Visibility.Visible);

            AnimateWidth(SIDEBAR_CLOSED, SIDEBAR_OPEN, 180, () =>
            {
                _isAnimating = false;

                if (_activeSubMenuButton != null)
                {
                    SetSubMenuActive(_activeSubMenuButton);
                }
            });

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += (s, e2) =>
            {
                timer.Stop();
                FadeIn(YearText, 150);
                FadeIn(YearChevron, 150);
                FadeInAllTexts();
            };
            timer.Start();
        }
        private void AnimateClose()
        {
            _isAnimating = true;

            var rotation = FindArrowRotation(BtnToggleSidebar);
            rotation?.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(0, TimeSpan.FromMilliseconds(180)) { EasingFunction = new CircleEase() });

            var arrow = FindChild<SvgViewbox>(BtnToggleSidebar, "ToggleArrowIcon");
            if (arrow != null) arrow.Source = new Uri("/Assets/Icons/chevron-right.svg", UriKind.Relative);

            FadeOut(YearText, 100);
            FadeOut(YearChevron, 100);
            FadeOutAllTexts();

            foreach (var sub in _subMenus.Values)
                sub.Visibility = Visibility.Collapsed;

            if (_activeMenuButton != null)
            {
                string activeTag = _activeMenuButton.Tag as string ?? GetMenuTagFromButton(_activeMenuButton);
                UpdateMenuCornerRadius(_activeMenuButton, activeTag);
            }

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
            timer.Tick += (s, e2) =>
            {
                timer.Stop();
                YearChevron.Visibility = Visibility.Collapsed;
                SetAllTextsVisibility(Visibility.Collapsed);

                AnimateWidth(SIDEBAR_OPEN, SIDEBAR_CLOSED, 180, () =>
                {
                    _isAnimating = false;
                });
            };
            timer.Start();
        }

        private void AnimateWidth(double from, double to, int durationMs, Action onComplete)
        {
            int steps = 30;
            double stepDuration = (double)durationMs / steps;
            int currentStep = 0;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(stepDuration) };
            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = EaseInOut((double)currentStep / steps);
                double currentWidth = from + (to - from) * progress;

                SidebarWidthChanged?.Invoke(currentWidth);

                if (currentStep >= steps)
                {
                    timer.Stop();
                    SidebarWidthChanged?.Invoke(to);
                    onComplete?.Invoke();
                }
            };
            timer.Start();
        }

        private double EaseInOut(double t)
        {
            if (t < 0.5) return 4 * t * t * t;
            return 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        private void ShowElements()
        {
            YearChevron.Visibility = Visibility.Visible;
            SetAllTextsVisibility(Visibility.Visible);

            FadeIn(YearText);
            FadeIn(YearChevron);
            FadeInAllTexts();
            SetAllTextsVisibility(Visibility.Visible);
        }

        private void HideElements()
        {
            FadeOut(YearText);
            FadeOut(YearChevron);
            FadeOutAllTexts();
            SetAllTextsVisibility(Visibility.Collapsed);
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

        private void FadeInAllTexts()
        {
            FadeInChild<TextBlock>(BtnDashboard, "txtDashboard");
            FadeInChild<SvgViewbox>(BtnDashboard, "chevDashboard");
            FadeInChild<TextBlock>(BtnProducts, "txtProducts");
            FadeInChild<SvgViewbox>(BtnProducts, "chevProducts");
            FadeInChild<TextBlock>(BtnPersons, "txtPersons");
            FadeInChild<SvgViewbox>(BtnPersons, "chevPersons");
            FadeInChild<TextBlock>(BtnBusinessInfo, "txtBusinessInfo"); FadeInChild<SvgViewbox>(BtnBusinessInfo, "chevBusinessInfo"); FadeInChild<TextBlock>(BtnWarehouse, "txtWarehouse");
            FadeInChild<SvgViewbox>(BtnWarehouse, "chevWarehouse");
            FadeInChild<TextBlock>(BtnBank, "txtBank");
            FadeInChild<SvgViewbox>(BtnBank, "chevBank");
            FadeInChild<TextBlock>(BtnSales, "txtSales");
            FadeInChild<SvgViewbox>(BtnSales, "chevSales");
            FadeInChild<TextBlock>(BtnReports, "txtReports");
            FadeInChild<SvgViewbox>(BtnReports, "chevReports");
        }


        private void FadeOutAllTexts()
        {
            FadeOutChild<TextBlock>(BtnDashboard, "txtDashboard");
            FadeOutChild<SvgViewbox>(BtnDashboard, "chevDashboard");
            FadeOutChild<TextBlock>(BtnProducts, "txtProducts");
            FadeOutChild<SvgViewbox>(BtnProducts, "chevProducts");
            FadeOutChild<TextBlock>(BtnPersons, "txtPersons");
            FadeOutChild<SvgViewbox>(BtnPersons, "chevPersons");
            FadeOutChild<TextBlock>(BtnBusinessInfo, "txtBusinessInfo"); FadeOutChild<SvgViewbox>(BtnBusinessInfo, "chevBusinessInfo"); FadeOutChild<TextBlock>(BtnWarehouse, "txtWarehouse");
            FadeOutChild<SvgViewbox>(BtnWarehouse, "chevWarehouse");
            FadeOutChild<TextBlock>(BtnBank, "txtBank");
            FadeOutChild<SvgViewbox>(BtnBank, "chevBank");
            FadeOutChild<TextBlock>(BtnSales, "txtSales");
            FadeOutChild<SvgViewbox>(BtnSales, "chevSales");
            FadeOutChild<TextBlock>(BtnReports, "txtReports");
            FadeOutChild<SvgViewbox>(BtnReports, "chevReports");
        }

        private void FadeInChild<T>(Button btn, string name) where T : FrameworkElement
        { var el = FindChild<T>(btn, name); if (el != null) FadeIn(el); }

        private void FadeOutChild<T>(Button btn, string name) where T : FrameworkElement
        { var el = FindChild<T>(btn, name); if (el != null) FadeOut(el); }

        private void SetAllTextsVisibility(Visibility vis)
        {
            SetVis<TextBlock>(BtnDashboard, "txtDashboard", vis);
            SetVis<SvgViewbox>(BtnDashboard, "chevDashboard", vis);
            SetVis<TextBlock>(BtnProducts, "txtProducts", vis);
            SetVis<SvgViewbox>(BtnProducts, "chevProducts", vis);
            SetVis<TextBlock>(BtnPersons, "txtPersons", vis);
            SetVis<SvgViewbox>(BtnPersons, "chevPersons", vis);
            SetVis<TextBlock>(BtnBusinessInfo, "txtBusinessInfo", vis); SetVis<SvgViewbox>(BtnBusinessInfo, "chevBusinessInfo", vis); SetVis<TextBlock>(BtnWarehouse, "txtWarehouse", vis);
            SetVis<SvgViewbox>(BtnWarehouse, "chevWarehouse", vis);
            SetVis<TextBlock>(BtnBank, "txtBank", vis);
            SetVis<SvgViewbox>(BtnBank, "chevBank", vis);
            SetVis<TextBlock>(BtnSales, "txtSales", vis);
            SetVis<SvgViewbox>(BtnSales, "chevSales", vis);
            SetVis<TextBlock>(BtnReports, "txtReports", vis);
            SetVis<SvgViewbox>(BtnReports, "chevReports", vis);
        }
        private void SetVis<T>(Button btn, string name, Visibility vis) where T : FrameworkElement
        { var el = FindChild<T>(btn, name); if (el != null) el.Visibility = vis; }

        private void ExpanderClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                SetActiveMenu(btn, tag);

                if (_activeSubMenuButton != null)
                {
                    SetSubMenuInactive(_activeSubMenuButton);
                    _activeSubMenuButton = null;
                }

                if (!_isSidebarOpen)
                {
                    _isSidebarOpen = true;
                    AnimateOpen();
                    var delay = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
                    delay.Tick += (s2, e2) => { delay.Stop(); ToggleSubMenu(btn, tag); };
                    delay.Start();
                    return;
                }
                ToggleSubMenu(btn, tag);
            }
        }
        private void ToggleSubMenu(Button btn, string tag)
        {
            if (_isSubMenuAnimating) return;
            _isSubMenuAnimating = true;

            if (!_subMenus.ContainsKey(tag)) return;
            var targetSub = _subMenus[tag];
            bool isOpen = targetSub.Visibility == Visibility.Visible;

            if (isOpen)
            {
                SmoothClose(targetSub, () =>
{
    UpdateMenuCornerRadius(btn, tag);
});
                SetChevron(btn, tag, "down");
                RotateSubMenuArrow(btn, 0);
                UpdateMenuCornerRadius(btn, tag);
            }
            else
            {
                StackPanel currentOpen = null;
                string currentOpenTag = null;

                foreach (var kvp in _subMenus)
                {
                    if (kvp.Value.Visibility == Visibility.Visible)
                    {
                        currentOpen = kvp.Value;
                        currentOpenTag = kvp.Key;
                        break;
                    }
                }

                ResetAllChevrons();

                if (currentOpen != null)
                {
                    SmoothClose(currentOpen, () =>
                    {
                        SmoothOpen(targetSub);
                        SetChevron(btn, tag, "up");
                        RotateSubMenuArrow(btn, 180);
                        UpdateMenuCornerRadius(btn, tag);
                    });
                }
                else
                {
                    SmoothOpen(targetSub);
                    SetChevron(btn, tag, "up");
                    RotateSubMenuArrow(btn, 180);
                    UpdateMenuCornerRadius(btn, tag);
                }
            }
        }
        private void ResetAllChevrons()
        {
            SetChevron(BtnDashboard, "dashboard", "down");
            SetChevron(BtnProducts, "products", "down");
            SetChevron(BtnPersons, "persons", "down");
            SetChevron(BtnBusinessInfo, "businessinfo", "down"); SetChevron(BtnWarehouse, "warehouse", "down");
            SetChevron(BtnBank, "bank", "down");
            SetChevron(BtnSales, "sales", "down");
            SetChevron(BtnReports, "reports", "down");
        }
        private void RotateArrow(double angle)
        {
            var rotation = FindArrowRotation(BtnToggleSidebar);
            if (rotation != null)
            {
                rotation.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(angle, TimeSpan.FromMilliseconds(180)) { EasingFunction = new CircleEase() });
            }
        }

        private void RotateArrowBack()
        {
            var rotation = FindArrowRotation(BtnToggleSidebar);
            if (rotation != null)
            {
                rotation.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(0, TimeSpan.FromMilliseconds(180)) { EasingFunction = new CircleEase() });
            }
        }
        private void SetChevron(Button btn, string tag, string direction)
        {
            string chevronName = "chev" + char.ToUpper(tag[0]) + tag.Substring(1);
            var chevron = FindChild<SvgViewbox>(btn, chevronName);

            if (chevron?.RenderTransform is RotateTransform rt)
            {
                double targetAngle = (direction == "up") ? 180 : 0;

                rt.BeginAnimation(RotateTransform.AngleProperty, null);

                rt.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(targetAngle, TimeSpan.FromMilliseconds(180))
                    {
                        EasingFunction = new CircleEase()
                    });
            }
        }
        private void SmoothOpen(StackPanel panel)
        {
            panel.Visibility = Visibility.Visible;
            panel.Opacity = 0;

            panel.Measure(new Size(SIDEBAR_OPEN, double.PositiveInfinity));
            double targetHeight = panel.DesiredSize.Height;
            panel.MaxHeight = 0;

            int totalSteps = 18;
            int currentStep = 0;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(14) };
            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = SmoothStep((double)currentStep / totalSteps);
                panel.MaxHeight = targetHeight * progress;

                if (progress > 0.15)
                    panel.Opacity = Math.Min(1, (progress - 0.15) / 0.85);

                if (currentStep >= totalSteps)
                {
                    timer.Stop();
                    panel.MaxHeight = double.PositiveInfinity;
                    panel.Opacity = 1;

                    // فقط اینجا باید فالس بشه!
                    _isSubMenuAnimating = false;

                    RestoreActiveSubMenuStyle();
                }
            };
            timer.Start();

            // این دو خط پایین رو حتماً پاک کنید چون باعث باگ میشن
            // _isSubMenuAnimating = false; 
            // panel.Visibility = Visibility.Visible;
            // panel.IsHitTestVisible = true;
        }

        private void RestoreActiveSubMenuStyle()
        {
            if (_activeSubMenuTag != null)
            {
                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();

                    foreach (var subPanel in _subMenus.Values)
                    {
                        if (subPanel.Visibility == Visibility.Visible)
                        {
                            var button = FindButtonByTag(subPanel, _activeSubMenuTag);
                            if (button != null)
                            {
                                _activeSubMenuButton = button;
                                SetSubMenuActive(button);
                                break;
                            }
                        }
                    }
                };
                timer.Start();
            }
        }
        private Button FindButtonByTag(DependencyObject parent, string tag)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Button btn && btn.Tag as string == tag)
                {
                    return btn;
                }

                var result = FindButtonByTag(child, tag);
                if (result != null)
                    return result;
            }
            return null;
        }
        private bool IsButtonInPanel(Button button, StackPanel panel)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(button);
            while (parent != null)
            {
                if (parent == panel)
                    return true;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return false;
        }

        private void SmoothClose(StackPanel panel, Action onComplete = null)
        {
            if (panel.Visibility != Visibility.Visible)
            {
                onComplete?.Invoke();
                return;
            }

            double startHeight = panel.ActualHeight;
            if (startHeight <= 0)
            {
                panel.Visibility = Visibility.Collapsed;
                onComplete?.Invoke();
                return;
            }

            int totalSteps = 12;
            int currentStep = 0;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(14) };

            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = SmoothStep((double)currentStep / totalSteps);
                panel.MaxHeight = startHeight * (1 - progress);
                panel.Opacity = Math.Max(0, 1 - progress * 1.3);

                if (currentStep >= totalSteps)
                {
                    timer.Stop();
                    panel.Visibility = Visibility.Collapsed;
                    panel.MaxHeight = double.PositiveInfinity;
                    panel.Opacity = 1;

                    // فقط اینجا باید فالس بشه!
                    _isSubMenuAnimating = false;

                    onComplete?.Invoke();
                }
            };
            timer.Start();

            // این خط رو حتماً پاک کنید!
            // _isSubMenuAnimating = false;
        }
        private double SmoothStep(double t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return t * t * (3 - 2 * t);
        }

        private bool _isYearPopupOpen = false;
        private int _selectedYear = 1404;
        private readonly int[] _years = { 1404, 1403, 1402 };

        private void AnimateCornerRadius(Border border, CornerRadius targetRadius, int durationMs = 90, Action onComplete = null)
        {
            var startRadius = border.CornerRadius;

            if (startRadius == targetRadius)
            {
                onComplete?.Invoke();
                return;
            }

            int totalSteps = 5;
            int interval = durationMs / totalSteps;
            int currentStep = 0;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(interval) };

            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = SmoothStep((double)currentStep / totalSteps);

                border.CornerRadius = new CornerRadius(
                    startRadius.TopLeft + (targetRadius.TopLeft - startRadius.TopLeft) * progress,
                    startRadius.TopRight + (targetRadius.TopRight - startRadius.TopRight) * progress,
                    startRadius.BottomRight + (targetRadius.BottomRight - startRadius.BottomRight) * progress,
                    startRadius.BottomLeft + (targetRadius.BottomLeft - startRadius.BottomLeft) * progress
                );

                if (currentStep >= totalSteps)
                {
                    timer.Stop();
                    border.CornerRadius = targetRadius;
                    onComplete?.Invoke();
                }
            };
            timer.Start();
        }
        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                SetActiveMenu(btn, "dashboard");
            }

            if (_activeSubMenuButton != null)
            {
                SetSubMenuInactive(_activeSubMenuButton);
                _activeSubMenuButton = null;
            }

            StackPanel openMenu = null;
            foreach (var kvp in _subMenus)
                if (kvp.Value.Visibility == Visibility.Visible)
                { openMenu = kvp.Value; break; }

            ResetAllChevrons();
            if (openMenu != null) SmoothClose(openMenu);
        }

        private void SubMenuClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;
            if (btn.Tag == null) return;

            string tag = btn.Tag.ToString();

            SubMenuClicked?.Invoke(tag);
        }

        private void AnimateSizeWithBounce(Ellipse ellipse, double from, double to, int durationMs)
        {
            int totalSteps = 20;
            int currentStep = 0;
            double stepDuration = (double)durationMs / totalSteps;
            double overshoot = to + (to - from) * 0.3;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(stepDuration) };
            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = (double)currentStep / totalSteps;
                double currentSize;

                if (progress < 0.6)
                {
                    double subProgress = progress / 0.6;
                    currentSize = from + (overshoot - from) * EaseOutQuad(subProgress);
                }
                else
                {
                    double subProgress = (progress - 0.6) / 0.4;
                    currentSize = overshoot + (to - overshoot) * EaseOutQuad(subProgress);
                }

                ellipse.Width = currentSize;
                ellipse.Height = currentSize;

                if (currentStep >= totalSteps)
                {
                    timer.Stop();
                    ellipse.Width = to;
                    ellipse.Height = to;
                }
            };
            timer.Start();
        }
        private void SetSubMenuInactive(Button btn)
        {
            btn.ApplyTemplate();

            var textBlock = btn.Template.FindName("PART_Text", btn) as TextBlock;
            var ellipse = btn.Template.FindName("PART_Dot", btn) as Ellipse;

            if (textBlock != null)
            {
                var currentColor = (textBlock.Foreground as SolidColorBrush)?.Color
                                   ?? (Color)ColorConverter.ConvertFromString("#737791");
                var newBrush = new SolidColorBrush(currentColor);
                textBlock.Foreground = newBrush;

                var anim = new ColorAnimation
                {
                    To = (Color)ColorConverter.ConvertFromString("#737791"),
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                anim.Completed += (s, e) =>
{
    textBlock.ClearValue(TextBlock.ForegroundProperty);
};

                newBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);

                AnimateFontSize(textBlock, 14, 13, 200);
                textBlock.FontWeight = FontWeights.Medium;
            }

            if (ellipse != null)
            {
                var currentColor = (ellipse.Fill as SolidColorBrush)?.Color
                                   ?? (Color)ColorConverter.ConvertFromString("#737791");
                var newFill = new SolidColorBrush(currentColor);
                ellipse.Fill = newFill;

                var anim = new ColorAnimation
                {
                    To = (Color)ColorConverter.ConvertFromString("#737791"),
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                anim.Completed += (s, e) =>
{
    ellipse.ClearValue(Shape.FillProperty);
};

                newFill.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                AnimateSize(ellipse, 8, 5, 200);
            }
        }


        private void YearSelector_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isYearPopupOpen)
                CloseYearPopup();
            else
                OpenYearPopup();
        }
        private void OpenYearPopup()
        {
            BuildYearList();
            _isYearPopupOpen = true;
            YearPopup.IsOpen = true;
            RotateYearChevron(180);
        }
        private void CloseYearPopup()
        {
            _isYearPopupOpen = false;
            YearPopup.IsOpen = false;
            RotateYearChevron(0);
        }
        private void RotateYearChevron(double angle)
        {
            YearChevronRotation.BeginAnimation(RotateTransform.AngleProperty,
                new DoubleAnimation(angle, TimeSpan.FromMilliseconds(180))
                { EasingFunction = new CircleEase() });
        }
        private void BuildYearList()
        {
            YearList.Children.Clear();

            foreach (var year in _years)
            {
                bool isSelected = year == _selectedYear;

                var btn = new Button
                {
                    Content = $"سال مالی {ToPersianDigits(year)}",
                    Height = 36,
                    Cursor = Cursors.Hand,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Background = isSelected
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E9F0FF"))
                        : Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    FontFamily = (FontFamily)FindResource("IRANSans"),
                    FontSize = 13,
                    FontWeight = isSelected ? FontWeights.Bold : FontWeights.Medium,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        isSelected ? "#0D2159" : "#737791")),
                    Padding = new Thickness(10, 0, 10, 0),
                    Tag = year
                };

                btn.MouseEnter += (s, e) =>
                {
                    var b = (Button)s;
                    if ((int)b.Tag != _selectedYear)
                        b.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F0F5FF"));
                };

                btn.MouseLeave += (s, e) =>
                {
                    var b = (Button)s;
                    b.Background = (int)b.Tag == _selectedYear
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E9F0FF"))
                        : Brushes.Transparent;
                };

                btn.Click += (s, e) =>
                {
                    _selectedYear = (int)((Button)s).Tag;
                    YearText.Text = $"سال مالی {ToPersianDigits(_selectedYear)}";
                    CloseYearPopup();
                };

                YearList.Children.Add(btn);
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
        private void SetSubMenuActive(Button btn)
        {
            btn.ApplyTemplate();

            var textBlock = btn.Template.FindName("PART_Text", btn) as TextBlock;
            var ellipse = btn.Template.FindName("PART_Dot", btn) as Ellipse;

            if (textBlock != null)
            {
                var newBrush = new SolidColorBrush(
    (Color)ColorConverter.ConvertFromString("#737791"));
                textBlock.Foreground = newBrush;

                newBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                    new ColorAnimation
                    {
                        To = (Color)ColorConverter.ConvertFromString("#0D2159"),
                        Duration = TimeSpan.FromMilliseconds(200),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    });

                AnimateFontSize(textBlock, 13, 14, 200);
                textBlock.FontWeight = FontWeights.ExtraBold;
            }

            if (ellipse != null)
            {
                var newFill = new SolidColorBrush(
    (Color)ColorConverter.ConvertFromString("#737791"));
                ellipse.Fill = newFill;

                newFill.BeginAnimation(SolidColorBrush.ColorProperty,
                    new ColorAnimation
                    {
                        To = (Color)ColorConverter.ConvertFromString("#0D2159"),

                        Duration = TimeSpan.FromMilliseconds(200),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    });

                AnimateSizeWithBounce(ellipse, 5, 8, 300);
            }
        }

        private RotateTransform FindArrowRotation(Button button)
        {
            var arrow = button.Template.FindName("ToggleArrowIcon", button) as SvgViewbox;
            if (arrow?.RenderTransform is RotateTransform rt)
                return rt;

            arrow = FindChild<SvgViewbox>(button, "ToggleArrowIcon");
            if (arrow?.RenderTransform is RotateTransform rt2)
                return rt2;

            return null;
        }

        private double EaseOutQuad(double t)
        {
            return 1 - (1 - t) * (1 - t);
        }
        private void AnimateSize(Ellipse ellipse, double from, double to, int durationMs)
        {
            int totalSteps = 15;
            int currentStep = 0;
            double stepDuration = (double)durationMs / totalSteps;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(stepDuration) };
            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = EaseOutQuad((double)currentStep / totalSteps);
                double currentSize = from + (to - from) * progress;

                ellipse.Width = currentSize;
                ellipse.Height = currentSize;

                if (currentStep >= totalSteps)
                {
                    timer.Stop();
                    ellipse.Width = to;
                    ellipse.Height = to;
                }
            };
            timer.Start();
        }
        private void AnimateFontSize(TextBlock textBlock, double from, double to, int durationMs)
        {
            int totalSteps = 15;
            int currentStep = 0;
            double stepDuration = (double)durationMs / totalSteps;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(stepDuration) };
            timer.Tick += (s, e) =>
            {
                currentStep++;
                double progress = EaseOutQuad((double)currentStep / totalSteps);
                textBlock.FontSize = from + (to - from) * progress;

                if (currentStep >= totalSteps)
                {
                    timer.Stop();
                    textBlock.FontSize = to;
                }
            };
            timer.Start();
        }

        private void RotateSubMenuArrow(Button btn, double angle)
        {
            var tag = btn.Tag as string;
            if (string.IsNullOrEmpty(tag)) return;

            string chevronName = "chev" + char.ToUpper(tag[0]) + tag.Substring(1);
            var chevron = FindChild<SvgViewbox>(btn, chevronName);

            if (chevron?.RenderTransform is RotateTransform rt)
            {
                rt.BeginAnimation(RotateTransform.AngleProperty,
                    new DoubleAnimation(angle, TimeSpan.FromMilliseconds(180))
                    { EasingFunction = new CircleEase() });
            }
        }
        public static T FindChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (string.IsNullOrEmpty(childName) && child is T tNoName)
                    return tNoName;

                if (child is T t && t.Name == childName) return t;
                var found = FindChild<T>(child, childName);
                if (found != null) return found;
            }
            return null;
        }
    }
}