using SecureVault.Controls;
using SecureVault.Helpers;
using SecureVault.Models;
using SecureVault.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SecureVault.Views;

public partial class PasswordList : ContentPage
{
    private string _searchText;
    private bool _isSidebarOpen = false;
    public ObservableCollection<PasswordItem> Passwords { get; set; } = new();
    private List<PasswordItem> _allPasswords = new();
    public ICommand DeleteCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterPasswords();
            }
        }
    }
    public PasswordList(string pageTitle)
    {
        InitializeComponent();
        PageTitle.Text = pageTitle;
        Passwords = new ObservableCollection<PasswordItem>();
        DeleteCommand = new Command<PasswordItem>(async (item) => await DeletePasswordItemAsync(item));
        BindingContext = this;

        PasswordFormControl.FormClosed += PasswordFormControl_FormClosed;
        PasswordFormControl.OnPasswordSaved += PasswordFormControl_OnPasswordSaved;

        MessagingCenter.Subscribe<Sidebar, string>(this, "GroupDeleted", async (sender, deletedGroupTitle) =>
        {
            if (deletedGroupTitle == PageTitle.Text)
            {
                await Application.Current.MainPage.DisplayAlert("Deleted", $"The group \"{deletedGroupTitle}\" was deleted.", "OK");

                // Tell MainPage to reload
                MessagingCenter.Send(this, "CurrentGroupDeleted");

                // Optional: Clear this content to avoid showing outdated data
                Content = new Label
                {
                    Text = "This group was deleted.",
                    TextColor = Colors.Gray,
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
            }
        });

        LoadPasswords();
    }

    private async void OnPasswordSelected(object sender, SelectionChangedEventArgs e)
    {
        var selected = e.CurrentSelection.FirstOrDefault() as PasswordItem;
        if (selected != null)
        {
            var editableCopy = new PasswordItem
            {
                Id = selected.Id,
                Title = selected.Title,
                Username = selected.Username,
                Password = selected.Password,
                Url = selected.Url,
                Notes = selected.Notes,
                GroupTitle = selected.GroupTitle
            };

            PasswordFormControl.BindingContext = editableCopy;

            PasswordFormControl.Opacity = 0;
            PasswordFormControl.TranslationX = 50;
            PasswordFormControl.IsVisible = true;
            AddCloseLabel.Text = MaterialDesignIconFonts.Close;
            AddCloseLabel.TextColor = Colors.DarkRed;

            MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            await Task.WhenAll(
                PasswordFormControl.TranslateTo(0, 0, 200, Easing.CubicOut),
                PasswordFormControl.FadeTo(1, 200, Easing.CubicOut)
            );
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        ToggleSidebarForm();
    }

    private async void ToggleSidebarForm()
    {
        if (PasswordFormControl.IsVisible)
        {   _isSidebarOpen = false;
            await PasswordFormControl.FadeTo(0, 150, Easing.CubicIn);
            await PasswordFormControl.TranslateTo(50, 0, 150, Easing.CubicIn);
            PasswordFormControl.IsVisible = false;
            MainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Absolute);
            AddCloseLabel.Text = MaterialDesignIconFonts.Add;
            AddCloseLabel.TextColor = Colors.Green;
        }
        else
        {
            _isSidebarOpen = true;
            PasswordsCollection.SelectedItem = null;

            PasswordFormControl.BindingContext = new PasswordItem
            {
                GroupTitle = PageTitle.Text.Trim()
            };

            PasswordFormControl.Opacity = 0;
            PasswordFormControl.TranslationX = 50;
            MainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            await Task.Delay(10);

            PasswordFormControl.IsVisible = true;

            await Task.WhenAll(
                PasswordFormControl.TranslateTo(0, 0, 200, Easing.CubicOut),
                PasswordFormControl.FadeTo(1, 200, Easing.CubicOut)
            );

            AddCloseLabel.Text = MaterialDesignIconFonts.Close;
            AddCloseLabel.TextColor = Colors.DarkRed;
        }
    }

    private async void PasswordFormControl_FormClosed(object sender, EventArgs e)
    {
        ToggleSidebarForm();
    }

    private async void PasswordFormControl_OnPasswordSaved(object sender, PasswordItem e)
    {
        await LoadPasswords();
        ToggleSidebarForm();
    }

    private async Task LoadPasswords()
    {
        await DatabaseService.InitAsync();
        var allItems = await DatabaseService.GetAllPasswordItemsAsync();

        _allPasswords = allItems
            .Where(p => p.GroupTitle == PageTitle.Text)
            .ToList();

        FilterPasswords();
    }

    private async Task DeletePasswordItemAsync(PasswordItem item)
    {
        if (item == null) return;

        bool confirm = await Application.Current.MainPage.DisplayAlert("Delete", $"Are you sure you want to delete \"{item.Title}\"?", "Yes", "Cancel");
        if (!confirm) return;

        await DatabaseService.InitAsync();
        await DatabaseService.DeletePasswordItemAsync(item);

        _allPasswords.Remove(item); 
        FilterPasswords();          
    }


    private void FilterPasswords()
    {
        if (_allPasswords == null)
            return;

        var query = SearchText?.Trim();

        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allPasswords
            : _allPasswords.Where(p =>
                Matches(query, p.Title) ||
                Matches(query, p.Username)
            ).ToList();

        Passwords.Clear();
        foreach (var item in filtered)
            Passwords.Add(item);
    }

    private bool Matches(string query, string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return text.Contains(query, StringComparison.OrdinalIgnoreCase) ||
               words.Any(w => string.Equals(w, query, StringComparison.OrdinalIgnoreCase));
    }


}