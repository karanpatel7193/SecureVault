using SecureVault.Models;
using SecureVault.Services;

namespace SecureVault.Views;

public partial class AddGroup : ContentPage
{
	public AddGroup()
	{
		InitializeComponent();
	}

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var title = TitleEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Error", "Title cannot be empty", "OK");
            return;
        }

        var newItem = new MenuItemModel
        {
            Title = title,
            Icon = "web_icon.png",
            PageTypeName = "SecureVault.Views.PasswordList"
        };

        await DatabaseService.SaveMenuItemAsync(newItem);

        MessagingCenter.Send(this, "MenuItemAdded");

        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}