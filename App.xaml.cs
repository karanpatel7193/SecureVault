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
            if (Preferences.ContainsKey("user_id"))
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            //MainPage = new AppShell();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        private async void InitApp()
        {
            await DatabaseService.InitAsync();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("App is closing. Releasing resources...");

            // Close DB or release anything held in memory
            DatabaseService.CloseConnection(); // Make sure this method exists
        }
    }
}
