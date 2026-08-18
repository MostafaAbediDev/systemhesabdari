using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Taadol.Controls
{
    public partial class TreeComboBox : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource),
                typeof(IEnumerable<TreeComboItem>), typeof(TreeComboBox),
                new PropertyMetadata(null, (d, _) => ((TreeComboBox)d).RefreshFilter()));

        public IEnumerable<TreeComboItem> ItemsSource
        {
            get => (IEnumerable<TreeComboItem>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem),
                typeof(TreeComboItem), typeof(TreeComboBox),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) =>
                    {
                        var ctrl = (TreeComboBox)d;
                        ctrl.SelectedText = (e.NewValue as TreeComboItem)?.DisplayName;
                    }));

        public TreeComboItem SelectedItem
        {
            get => (TreeComboItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder),
                typeof(string), typeof(TreeComboBox),
                new PropertyMetadata("یک دسته انتخاب کنید..."));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen),
                typeof(bool), typeof(TreeComboBox),
                new PropertyMetadata(false, (d, e) =>
                {
                    if ((bool)e.NewValue)
                        ((TreeComboBox)d).Dispatcher.BeginInvoke(new Action(() =>
                            ((TreeComboBox)d).PART_SearchBox.Focus()),
                            System.Windows.Threading.DispatcherPriority.Input);
                }));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        
        public static readonly DependencyProperty AddRootCommandProperty =
            DependencyProperty.Register(nameof(AddRootCommand), typeof(ICommand), typeof(TreeComboBox));
        public ICommand AddRootCommand
        {
            get => (ICommand)GetValue(AddRootCommandProperty);
            set => SetValue(AddRootCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(TreeComboBox));
        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(TreeComboBox));
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty AddChildCommandProperty =
            DependencyProperty.Register(nameof(AddChildCommand), typeof(ICommand), typeof(TreeComboBox));
        public ICommand AddChildCommand
        {
            get => (ICommand)GetValue(AddChildCommandProperty);
            set => SetValue(AddChildCommandProperty, value);
        }

        #endregion

        #region Bindable Properties

        private string _selectedText;
        public string SelectedText
        {
            get => _selectedText;
            private set { _selectedText = value; OnPropertyChanged(); }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                RefreshFilter();
            }
        }

        private ObservableCollection<TreeComboItem> _filteredItems = new ObservableCollection<TreeComboItem>();
        public ObservableCollection<TreeComboItem> FilteredItems
        {
            get => _filteredItems;
            private set { _filteredItems = value; OnPropertyChanged(); }
        }

        #endregion

        #region Events
        public event EventHandler<TreeComboItem> SelectionChanged;
        #endregion

        #region Constructor
        public TreeComboBox()
        {
            InitializeComponent();
            PART_TreeView.SelectedItemChanged += OnTreeSelectedItemChanged;
        }
        #endregion

        #region Selection
        private void OnTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeComboItem item)
            {
                SelectedItem = item;
                SelectedText = item.DisplayName;
                IsDropDownOpen = false;
                SearchText = string.Empty;
                SelectionChanged?.Invoke(this, item);
            }
        }
        #endregion

        #region Search / Filter
        private void RefreshFilter()
        {
            var source = ItemsSource;
            if (source == null) { FilteredItems = new ObservableCollection<TreeComboItem>(); return; }

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredItems = new ObservableCollection<TreeComboItem>(source);
            }
            else
            {
                var filtered = FilterTree(source, SearchText.Trim()).ToList();
                ExpandAll(filtered);
                FilteredItems = new ObservableCollection<TreeComboItem>(filtered);
            }
        }

        private IEnumerable<TreeComboItem> FilterTree(IEnumerable<TreeComboItem> items, string q)
        {
            if (items == null) yield break;
            foreach (var item in items)
            {
                var kids = FilterTree(item.Children, q).ToList();
                if (item.DisplayName?.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 || kids.Any())
                {
                    var clone = new TreeComboItem(item.DisplayName, item.Value) { IsExpanded = kids.Any(), IsRoot = item.IsRoot };
                    clone.Children.AddRange(kids);
                    yield return clone;
                }
            }
        }

        private void ExpandAll(IEnumerable<TreeComboItem> items)
        {
            foreach (var item in items ?? Enumerable.Empty<TreeComboItem>())
            {
                if (item.Children.Any()) { item.IsExpanded = true; ExpandAll(item.Children); }
            }
        }
        #endregion

        #region Keyboard
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape) { IsDropDownOpen = false; e.Handled = true; }
            else if ((e.Key == Key.F4 || e.Key == Key.Down) && !IsDropDownOpen) { IsDropDownOpen = true; e.Handled = true; }
        }
        #endregion
    }
}