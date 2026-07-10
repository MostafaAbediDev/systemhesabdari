using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Taadol.Controls
{
    public enum ToastType { Success, Error, Warning, Info }

    public partial class ToastNotification : UserControl
    {
        private readonly DispatcherTimer _timer;
        private ICommand _closeCommand;

        public ToastNotification(string message, ToastType type, int durationMs = 3000)
        {
            InitializeComponent();

            MessageText.Text = message;

            _closeCommand = new RelayCommand(() =>
            {
                _timer.Stop();
                Dismiss();
            });
            CloseBorder.DataContext = this;

            switch (type)
            {
                case ToastType.Success:
                    IconCircle.Background = new SolidColorBrush(Color.FromRgb(220, 252, 231)); // green-50
                    IconInner.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));     // green-500
                    SetCheckIcon();
                    break;
                case ToastType.Error:
                    IconCircle.Background = new SolidColorBrush(Color.FromRgb(254, 226, 226)); // red-50
                    IconInner.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));     // red-500
                    SetCloseIcon();
                    break;
                case ToastType.Warning:
                    IconCircle.Background = new SolidColorBrush(Color.FromRgb(255, 247, 205)); // amber-50
                    IconInner.Background = new SolidColorBrush(Color.FromRgb(245, 158, 11));     // amber-500
                    SetWarningIcon();
                    break;
                case ToastType.Info:
                    IconCircle.Background = new SolidColorBrush(Color.FromRgb(219, 234, 254)); // blue-50
                    IconInner.Background = new SolidColorBrush(Color.FromRgb(59, 130, 246));     // blue-500
                    SetInfoIcon();
                    break;
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(durationMs)
            };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                Dismiss();
            };
        }

        public ICommand CloseCommand => _closeCommand;

        public void Show()
        {
            _timer.Start();

            var fadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
            var slideIn = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            SlideTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);
            BeginAnimation(OpacityProperty, fadeIn);
        }

        public void Dismiss()
        {
            var fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            var slideOut = new DoubleAnimation(-60, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            slideOut.Completed += (s, e) =>
            {
                ((Panel)Parent)?.Children.Remove(this);
            };

            BeginAnimation(OpacityProperty, fadeOut);
            SlideTransform.BeginAnimation(TranslateTransform.YProperty, slideOut);
        }

        private void SetCheckIcon()
        {
            var canvas = new Canvas { Width = 18, Height = 18 };
            canvas.Children.Add(new System.Windows.Shapes.Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 2.2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round,
                Data = Geometry.Parse("M 3,9 L 7,13 L 15,5")
            });
            IconSvg.Child = canvas;
        }

        private void SetCloseIcon()
        {
            var canvas = new Canvas { Width = 18, Height = 18 };
            canvas.Children.Add(new System.Windows.Shapes.Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 2.2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Data = Geometry.Parse("M 4,4 L 14,14 M 14,4 L 4,14")
            });
            IconSvg.Child = canvas;
        }

        private void SetWarningIcon()
        {
            var canvas = new Canvas { Width = 18, Height = 18 };
            canvas.Children.Add(new System.Windows.Shapes.Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 1.8,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Data = Geometry.Parse("M 9,2 L 17,16 L 1,16 Z")
            });
            var dot = new System.Windows.Shapes.Ellipse
            {
                Fill = Brushes.White,
                Width = 2, Height = 2
            };
            Canvas.SetLeft(dot, 8);
            Canvas.SetTop(dot, 7);
            canvas.Children.Add(dot);
            var line = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Data = Geometry.Parse("M 9,10 L 9,13")
            };
            canvas.Children.Add(line);
            IconSvg.Child = canvas;
        }

        private void SetInfoIcon()
        {
            var canvas = new Canvas { Width = 18, Height = 18 };
            var circle = new System.Windows.Shapes.Ellipse
            {
                Stroke = Brushes.White,
                StrokeThickness = 1.6,
                Width = 15, Height = 15
            };
            Canvas.SetLeft(circle, 1.5);
            Canvas.SetTop(circle, 1.5);
            canvas.Children.Add(circle);
            var dot = new System.Windows.Shapes.Ellipse
            {
                Fill = Brushes.White,
                Width = 2, Height = 2
            };
            Canvas.SetLeft(dot, 8);
            Canvas.SetTop(dot, 4);
            canvas.Children.Add(dot);
            canvas.Children.Add(new System.Windows.Shapes.Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 2,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Data = Geometry.Parse("M 9,7 L 9,13")
            });
            IconSvg.Child = canvas;
        }
    }

    internal class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
}
