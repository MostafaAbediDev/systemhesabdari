using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaadolAccounting.Models;

namespace Taadol.ViewModels
{
    public class SidebarViewModel : INotifyPropertyChanged
    {
        private bool _isSidebarOpen = true;
        private string _selectedYear = "۱۴۰۴";

        public SidebarViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Title = "داشبورد مدیریت",
                    IconPath = "/Assets/Icons/calendar-tick.svg",
                },
                new MenuItem
                {
                    Title = "کالاها و خدمات",
                    IconPath = "/Assets/Icons/products.svg",
                    IsExpanded = true,
                    IsSelected = true,
                    SubItems = new ObservableCollection<SubMenuItem>
                    {
                        new SubMenuItem { Title = "لیست کالاها و خدمات" },
                        new SubMenuItem { Title = "کالای جدید", IsSelected = true, IsBold = true },
                        new SubMenuItem { Title = "خدمات جدید" }
                    }
                },
                new MenuItem
                {
                    Title = "اشخاص",
                    IconPath = "/Assets/Icons/people.svg",
                    SubItems = new ObservableCollection<SubMenuItem>
                    {
                        new SubMenuItem { Title = "لیست اشخاص" },
                        new SubMenuItem { Title = "شخص جدید" }
                    }
                },
                new MenuItem
                {
                    Title = "انبار",
                    IconPath = "/Assets/Icons/warehouse.svg",
                    SubItems = new ObservableCollection<SubMenuItem>
                    {
                        new SubMenuItem { Title = "لیست انبارها" },
                        new SubMenuItem { Title = "رسید انبار" },
                        new SubMenuItem { Title = "حواله انبار" }
                    }
                },
                new MenuItem
                {
                    Title = "بانک ها و صندوق",
                    IconPath = "/Assets/Icons/bank.svg",
                    SubItems = new ObservableCollection<SubMenuItem>
                    {
                        new SubMenuItem { Title = "لیست بانک‌ها" },
                        new SubMenuItem { Title = "صندوق" }
                    }
                },
                new MenuItem
                {
                    Title = "عملیات خرید و فروش",
                    IconPath = "/Assets/Icons/transaction.svg",
                    SubItems = new ObservableCollection<SubMenuItem>
                    {
                        new SubMenuItem { Title = "فاکتور خرید" },
                        new SubMenuItem { Title = "فاکتور فروش" },
                        new SubMenuItem { Title = "مرجوعی" }
                    }
                },
                new MenuItem
                {
                    Title = "گزارش ها",
                    IconPath = "/Assets/Icons/reports.svg",
                    SubItems = new ObservableCollection<SubMenuItem>
                    {
                        new SubMenuItem { Title = "گزارش فروش" },
                        new SubMenuItem { Title = "گزارش خرید" },
                        new SubMenuItem { Title = "سود و زیان" }
                    }
                }
            };

            Years = new ObservableCollection<string> { "۱۴۰۲", "۱۴۰۳", "۱۴۰۴" };

            ToggleSidebarCommand = new RelayCommand(o => IsSidebarOpen = !IsSidebarOpen);
            ToggleMenuCommand = new RelayCommand(ToggleMenu);
            SelectSubItemCommand = new RelayCommand(SelectSubItem);
        }

        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<string> Years { get; set; }

        public bool IsSidebarOpen
        {
            get => _isSidebarOpen;
            set
            {
                _isSidebarOpen = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SidebarWidth));
            }
        }

        public double SidebarWidth => IsSidebarOpen ? 240 : 60;

        public string SelectedYear
        {
            get => _selectedYear;
            set { _selectedYear = value; OnPropertyChanged(); }
        }

        public ICommand ToggleSidebarCommand { get; }
        public ICommand ToggleMenuCommand { get; }
        public ICommand SelectSubItemCommand { get; }

        private void ToggleMenu(object parameter)
        {
            if (parameter is MenuItem menuItem)
            {
                foreach (var item in MenuItems)
                {
                    if (item != menuItem)
                        item.IsExpanded = false;
                }

                menuItem.IsExpanded = !menuItem.IsExpanded;

                if (!IsSidebarOpen && menuItem.HasSubItems)
                    IsSidebarOpen = true;
            }
        }

        private void SelectSubItem(object parameter)
        {
            if (parameter is SubMenuItem subItem)
            {
                foreach (var menu in MenuItems)
                {
                    menu.IsSelected = false;
                    foreach (var sub in menu.SubItems)
                    {
                        sub.IsSelected = false;
                        sub.IsBold = false;
                    }
                }

                subItem.IsSelected = true;
                subItem.IsBold = true;

                foreach (var menu in MenuItems)
                {
                    foreach (var sub in menu.SubItems)
                    {
                        if (sub == subItem)
                        {
                            menu.IsSelected = true;
                            menu.IsExpanded = true;
                            break;
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
