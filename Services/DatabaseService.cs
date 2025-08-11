using SecureVault.Models;
using SQLite;

namespace SecureVault.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;
        private static bool _initialized;

        public static async Task InitAsync()
        {
            if (_initialized) return;

            var path = Path.Combine(FileSystem.AppDataDirectory, "securevault.db");
            _database = new SQLiteAsyncConnection(path);

            await _database.CreateTableAsync<MenuItemModel>();
            await _database.CreateTableAsync<PasswordItem>();
            await _database.CreateTableAsync<User>();
            _initialized = true;
        }
        public static int GetCurrentUserId()
        {
            return Preferences.Get("user_id", 0);
        }

        public static Task<int> RegisterUserAsync(User user)
            => _database.InsertAsync(user);

        public static Task<User?> GetUserByCredentialsAsync(string username, string password)
            => _database.Table<User>()
                .Where(u => u.UserName == username && u.Password == password)
                .FirstOrDefaultAsync();

        public static Task<bool> UsernameExistsAsync(string username)
            => _database.Table<User>()
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync()
                .ContinueWith(task => task.Result != null);

        public static Task<List<MenuItemModel>> GetMenuItemsAsync()
        {
            var userId = GetCurrentUserId();
            return _database.Table<MenuItemModel>()
                            .Where(m => m.UserId == userId)
                            .ToListAsync();
        }

        public static Task<int> SaveMenuItemAsync(MenuItemModel item)
        {
            item.UserId = GetCurrentUserId();
            return item.Id != 0 ? _database.UpdateAsync(item) : _database.InsertAsync(item);
        }

        public static Task<int> DeleteMenuItemAsync(MenuItemModel item)
            => _database.DeleteAsync(item);

        public static Task<int> SavePasswordItemAsync(PasswordItem item)
        {
            item.UserId = GetCurrentUserId();
            return item.Id != 0 ? _database.UpdateAsync(item) : _database.InsertAsync(item);
        }

        public static Task<List<PasswordItem>> GetAllPasswordItemsAsync()
        {
            var userId = GetCurrentUserId();
            return _database.Table<PasswordItem>()
                            .Where(p => p.UserId == userId)
                            .ToListAsync();
        }

        public static async Task<Dictionary<string, List<PasswordItem>>> GetGroupedPasswordItemsAsync()
        {
            var allItems = await GetAllPasswordItemsAsync();
            return allItems
                .GroupBy(item => item.GroupTitle ?? "Ungrouped")
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public static Task<int> DeletePasswordItemAsync(PasswordItem item)
            => _database.DeleteAsync(item);

        public static void CloseConnection()
        {
            try
            {
                _database?.GetConnection().Close();
                _database = null;
                Console.WriteLine("Database connection closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing DB: " + ex.Message);
            }
        }
    }
}
