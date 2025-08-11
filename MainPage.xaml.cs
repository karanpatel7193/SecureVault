using SecureVault.Models;
using Microsoft.Maui.Controls;
using SecureVault.Views;
using SecureVault.Services;
using System.Collections.ObjectModel;

namespace SecureVault
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<MenuItemModel> MenuItemsList { get; set; }
        private bool _isSidebarExpanded = true;
        private double _expandedWidth = 250;
        private double _collapsedWidth = 60;

        public bool IsSidebarExpanded
        {
            get => _isSidebarExpanded;
            set
            {
                _isSidebarExpanded = value;
                OnPropertyChanged(nameof(IsSidebarExpanded));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            InitializeAsync();
            MessagingCenter.Subscribe<AddGroup>(this, "MenuItemAdded", async (sender) =>
            {
                await LoadMenusAsync();
            });
        }

        // Separate async initialization method
        private async void InitializeAsync()
        {
            await LoadMenusAsync();
        }

        public async Task LoadMenusAsync()
        {
            await DatabaseService.InitAsync();

            var items = await DatabaseService.GetMenuItemsAsync();
            MenuItemsList = new ObservableCollection<MenuItemModel>(items);
            SidebarControl.ItemsSource = MenuItemsList;

            if (MenuItemsList.Any())
            {
                LoadPage(MenuItemsList[0]);
            }
            else
            {
                ContentArea.Content = new Label
                {
                    Text = "No menu items available.",
                    TextColor = Colors.Gray,
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
            }
        }


        private async void HamburgerButton_Clicked(object sender, EventArgs e)
        {
            IsSidebarExpanded = !IsSidebarExpanded;

            double targetWidth = IsSidebarExpanded ? _expandedWidth : _collapsedWidth;
            await Sidebar.WidthRequestTo(targetWidth, 250);
        }

        private void MenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is MenuItemModel selectedItem)
            {
                LoadPage(selectedItem);
            }
        }

        private async void LoadPage(MenuItemModel menuPage)
        {
            if (menuPage?.PageType == null)
            {
                ContentArea.Content = new Label
                {
                    Text = "Invalid page data.",
                    TextColor = Colors.Red,
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                return;
            }

            var page = Activator.CreateInstance(menuPage.PageType, menuPage.Title) as ContentPage;

            if (page != null)
            {
                ContentArea.Content = page.Content;
                ContentArea.BindingContext = page.BindingContext;
            }
            else
            {
                ContentArea.Content = new Label
                {
                    Text = "Error loading page.",
                    TextColor = Colors.Red,
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
            }
        }
    }

    public static class ViewExtensions
    {
        public static async Task WidthRequestTo(this VisualElement view, double newWidth, uint length = 250, Easing easing = null)
        {
            if (view == null)
                return;

            var tcs = new TaskCompletionSource<bool>();

            var originalWidth = view.WidthRequest;
            easing ??= Easing.Linear;

            var animation = new Animation(d => view.WidthRequest = d, originalWidth, newWidth);

            animation.Commit(view, "WidthRequestTo", 16, length, easing, (v, c) => tcs.SetResult(c));

            await tcs.Task;
        }
    }

}
