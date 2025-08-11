using SecureVault.Models;
using SecureVault.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SecureVault.Controls;

public partial class Sidebar : ContentView
{
    public ICommand DeleteMenuItemCommand { get; }
    public Sidebar()
	{
        InitializeComponent();

        DeleteMenuItemCommand = new Command<MenuItemModel>(async (item) =>
        {
            await DeleteMenuItemAsync(item);
        });

        BindingContext = this;
    }
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(ObservableCollection<MenuItemModel>), typeof(Sidebar), defaultBindingMode: BindingMode.TwoWay);

    public ObservableCollection<MenuItemModel> ItemsSource
    {
        get => (ObservableCollection<MenuItemModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectionChanged?.Invoke(this, e);
    }

    public event EventHandler HamburgerClicked;

    private void OnHamburgerClicked(object sender, EventArgs e)
    {
        HamburgerClicked?.Invoke(this, e);
    }
    private async void OnAddMenuClicked(object sender, EventArgs e)
    {
        var addGroup = new SecureVault.Views.AddGroup();
        await Application.Current.MainPage.Navigation.PushModalAsync(addGroup);
    }

    private async Task DeleteMenuItemAsync(MenuItemModel item)
    {
        if (item == null)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Delete Group", $"Delete \"{item.Title}\" and all its passwords?", "Yes", "No");

        if (confirm)
        {
            await DatabaseService.InitAsync();

            var allPasswords = await DatabaseService.GetAllPasswordItemsAsync();
            var relatedPasswords = allPasswords.Where(p => p.GroupTitle == item.Title).ToList();

            foreach (var password in relatedPasswords)
            {
                await DatabaseService.DeletePasswordItemAsync(password);
            }

            await DatabaseService.DeleteMenuItemAsync(item);

            ItemsSource?.Remove(item);

            MessagingCenter.Send(this, "GroupDeleted", item.Title);
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Logout", "Are you sure you want to logout?", "Yes", "No");

        if (confirm)
        {
            Preferences.Clear(); 

            Application.Current.MainPage = new NavigationPage(new SecureVault.Views.LoginPage());
        }
    }

}