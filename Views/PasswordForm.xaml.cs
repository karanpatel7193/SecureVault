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

}