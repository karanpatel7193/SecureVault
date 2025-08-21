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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        titleLabel.Opacity = 0;
        usernameEntry.Opacity = 0;
        passwordEntry.Opacity = 0;
        confirmPasswordEntry.Opacity = 0;
        registerButton.Opacity = 0;
        backToLoginButton.Opacity = 0;

        titleLabel.TranslationY = -40;
        usernameEntry.TranslationX = -100;
        passwordEntry.TranslationX = 100;
        confirmPasswordEntry.TranslationX = -100;
        registerButton.TranslationY = 40;
        backToLoginButton.TranslationY = 40;

        await titleLabel.TranslateTo(0, 0, 600, Easing.CubicOut);
        await titleLabel.FadeTo(1, 400);

        await Task.WhenAll(
            usernameEntry.TranslateTo(0, 0, 400, Easing.CubicOut),
            usernameEntry.FadeTo(1, 400)
        );

        await Task.WhenAll(
            passwordEntry.TranslateTo(0, 0, 400, Easing.CubicOut),
            passwordEntry.FadeTo(1, 400)
        );

        await Task.WhenAll(
            confirmPasswordEntry.TranslateTo(0, 0, 400, Easing.CubicOut),
            confirmPasswordEntry.FadeTo(1, 400)
        );

        await Task.WhenAll(
            registerButton.TranslateTo(0, 0, 400, Easing.CubicOut),
            registerButton.FadeTo(1, 400)
        );

        await Task.WhenAll(
            backToLoginButton.TranslateTo(0, 0, 400, Easing.CubicOut),
            backToLoginButton.FadeTo(1, 400)
        );
        AnimateBackground();
    }
    private async void AnimateBackground()
    {
        while (true)
        {
            await backgroundImage.ScaleTo(1.1, 10000, Easing.SinInOut);

            await Task.Delay(500);

            await backgroundImage.ScaleTo(1.0, 10000, Easing.SinInOut);

            await Task.Delay(500);
        }
    }

}