using CommunityToolkit.Maui.Views;
using SecureVault.Helpers;
using SecureVault.Models;
using SecureVault.Services;
using System.Reflection;

namespace SecureVault.Popups;

public partial class AddGroup : Popup
{
    public AddGroup()
    {
        InitializeComponent();
        this.Color = Colors.Transparent;
        Dispatcher.Dispatch(async () =>
        {
            PopupFrame.Opacity = 0;
            PopupFrame.Scale = 0.8;

            await PopupFrame.FadeTo(1, 200);
            await PopupFrame.ScaleTo(1, 200, Easing.SinInOut);
        });

        _ = LoadIconOptionsAsync();

    }

    private async Task LoadIconOptionsAsync()
    {
        var iconOptions = await Task.Run(() =>
        {
            return typeof(MaterialDesignIconFonts)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => new IconOption(f.Name.Replace('_', ' '), (string)f.GetValue(null)))
                .ToList();
        });

        MainThread.BeginInvokeOnMainThread(() =>
        {
            IconPicker.ItemsSource = iconOptions;

            IconPicker.SelectedIndexChanged += (s, e) =>
            {
                if (IconPicker.SelectedItem is IconOption selected)
                {
                    IconPreviewLabel.Text = selected.Value;
                }
            };
        });
    }
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var title = TitleEntry.Text?.Trim();
        var selectedIcon = IconPicker.SelectedItem as IconOption;

        if (string.IsNullOrWhiteSpace(title))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Title cannot be empty", "OK");
            return;
        }
        if (selectedIcon == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please select an icon", "OK");
            return;
        }

        var newItem = new MenuItemModel
        {
            Title = title,
            Icon = selectedIcon.Value,
            PageTypeName = "SecureVault.Views.PasswordList"
        };

        await DatabaseService.SaveMenuItemAsync(newItem);

        MessagingCenter.Send(this, "MenuItemAdded");

        await AnimateAndCloseAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await AnimateAndCloseAsync();
    }

    private async Task AnimateAndCloseAsync()
    {
        await PopupFrame.FadeTo(0, 150);
        await PopupFrame.ScaleTo(0.8, 150, Easing.SinInOut);
        Close(); 
    }

}
