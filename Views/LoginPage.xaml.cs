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
}