using SecureVault.Models;
using SecureVault.Services;

namespace SecureVault.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await DatabaseService.InitAsync();

        string username = usernameEntry.Text?.Trim();
        string password = passwordEntry.Text;
        string confirmPassword = confirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        bool userExists = await DatabaseService.UsernameExistsAsync(username);
        if (userExists)
        {
            await DisplayAlert("Error", "Username already exists.", "OK");
            return;
        }

        var user = new User
        {
            UserName = username,
            Password = password 
        };

        await DatabaseService.RegisterUserAsync(user);

        await DisplayAlert("Success", "Account created. Please login.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}