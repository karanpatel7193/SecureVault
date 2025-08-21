using SecureVault.Models;
using SecureVault.Services;
namespace SecureVault.Views;

public partial class PasswordForm : ContentView
{
    public event EventHandler<PasswordItem> OnPasswordSaved;
    public event EventHandler FormClosed;
    public PasswordForm()
    {
        InitializeComponent();
    }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is PasswordItem item)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Title is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(item.Url))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "URL is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(item.Username))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Username is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(item.Password))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Password is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(item.GroupTitle))
                item.GroupTitle = "Ungrouped";

            await DatabaseService.InitAsync();
            await DatabaseService.SavePasswordItemAsync(item);

            OnPasswordSaved?.Invoke(this, item);
            BindingContext = new PasswordItem();

            FormClosed?.Invoke(this, EventArgs.Empty);
        }
    }


    private async void OnCloseClicked(object sender, EventArgs e)
    {
        BindingContext = new PasswordItem();

        FormClosed?.Invoke(this, EventArgs.Empty);
    }

    private async void OnCopyPasswordClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await Clipboard.SetTextAsync(PasswordEntry.Text);
            await Application.Current.MainPage.DisplayAlert("Copied", "Password copied to clipboard.", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No password to copy.", "OK");
        }
    }
    private async void OnCopyUsernameClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(UsernameEntry.Text))
        {
            await Clipboard.SetTextAsync(UsernameEntry.Text);
            await Application.Current.MainPage.DisplayAlert("Copied", "Username copied to clipboard.", "OK");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No Username to copy.", "OK");
        }
    }


}