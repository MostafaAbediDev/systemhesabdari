using System.Windows;
using System.Windows.Controls;

namespace Taadol.Controls
{

    public partial class ListSummaryBar : UserControl
    {
        public ListSummaryBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedSummaryTextProperty =
            DependencyProperty.Register(nameof(SelectedSummaryText), typeof(string), typeof(ListSummaryBar),
                new PropertyMetadata("", (d, e) =>
                {
                    var control = (ListSummaryBar)d;
                    if (control.SelectedSummaryTextBlock != null)
                        control.SelectedSummaryTextBlock.Text = e.NewValue as string;
                }));

        public string SelectedSummaryText
        {
            get => (string)GetValue(SelectedSummaryTextProperty);
            set => SetValue(SelectedSummaryTextProperty, value);
        }

        public static readonly DependencyProperty TotalCountTextProperty =
            DependencyProperty.Register(nameof(TotalCountText), typeof(string), typeof(ListSummaryBar),
                new PropertyMetadata("", (d, e) =>
                {
                    var control = (ListSummaryBar)d;
                    if (control.TotalCountTextBlock != null)
                        control.TotalCountTextBlock.Text = e.NewValue as string;
                }));

        public string TotalCountText
        {
            get => (string)GetValue(TotalCountTextProperty);
            set => SetValue(TotalCountTextProperty, value);
        }
    }
}