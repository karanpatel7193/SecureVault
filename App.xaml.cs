using SecureVault.Services;
using SecureVault.Views;

namespace SecureVault
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitApp();

            Preferences.Remove("user_id");
            Preferences.Remove("username");

            MainPage = new NavigationPage(new LoginPage());

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        private async void InitApp()
        {
            await DatabaseService.InitAsync();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("App is closing. Releasing resources...");

            DatabaseService.CloseConnection();
        }
    }
}
