using SecureVault.Services;

namespace SecureVault.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await DatabaseService.InitAsync();

        string username = usernameEntry.Text?.Trim();
        string password = passwordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter both username and password.", "OK");
            return;
        }

        var user = await DatabaseService.GetUserByCredentialsAsync(username, password);
        if (user != null)
        {
            Preferences.Set("user_id", user.Id);
            Preferences.Set("username", user.UserName);

            await DisplayAlert("Success", $"Welcome, {user.UserName}!", "OK");
            Application.Current.MainPage = new AppShell(); ;
        }
        else
        {
            await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        titleLabel.Opacity = 0;
        usernameEntry.Opacity = 0;
        passwordEntry.Opacity = 0;
        loginButton.Opacity = 0;

        titleLabel.TranslationY = -40;
        usernameEntry.TranslationX = -100;
        passwordEntry.TranslationX = 100;
        loginButton.TranslationY = 40;

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
            loginButton.TranslateTo(0, 0, 400, Easing.CubicOut),
            loginButton.FadeTo(1, 400)
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