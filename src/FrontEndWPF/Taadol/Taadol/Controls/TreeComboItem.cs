using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Taadol.Controls
{
    public class TreeComboItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string DisplayName { get; set; }
        public object Value { get; set; }

                public bool IsRoot { get; set; }

        public List<TreeComboItem> Children { get; } = new List<TreeComboItem>();
        public bool HasChildren => Children.Count > 0;

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public TreeComboItem() { }
        public TreeComboItem(string displayName, object value = null)
        {
            DisplayName = displayName;
            Value = value;
        }

                public TreeComboItem AddChild(TreeComboItem child)
        {
            Children.Add(child);
            return this;
        }

        public TreeComboItem AddChild(string displayName, object value = null)
            => AddChild(new TreeComboItem(displayName, value));

        public override string ToString() => DisplayName ?? base.ToString();
    }
}