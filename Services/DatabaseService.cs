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

        public static async Task<int> RegisterUserAsync(User user)
        {
            try
            {
                if (await UsernameExistsAsync(user.UserName))
                {
                    throw new Exception("Username already exists.");
                }
                return await _database.InsertAsync(user);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public static async Task<User?> GetUserByCredentialsAsync(string username, string password)
        {
            try
            {
               return await _database.Table<User>()
                    .Where(u => u.UserName == username && u.Password == password)
                    .FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                return await _database.Table<User>().Where(u => u.UserName == username).FirstOrDefaultAsync().ContinueWith(task => task.Result != null);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<List<MenuItemModel>> GetMenuItemsAsync()
        {
            try
            {
            var userId = GetCurrentUserId();
            return await _database.Table<MenuItemModel>()
                            .Where(m => m.UserId == userId)
                            .ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<int> SaveMenuItemAsync(MenuItemModel item)
        {
            try
            {
                item.UserId = GetCurrentUserId();
                return item.Id != 0 ? await _database.UpdateAsync(item) : await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<int> DeleteMenuItemAsync(MenuItemModel item)
        {
            try
            {
                return await _database.DeleteAsync(item);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<int> SavePasswordItemAsync(PasswordItem item)
        {
            try
            {
                item.UserId = GetCurrentUserId();
                return item.Id != 0 ? await _database.UpdateAsync(item) : await _database.InsertAsync(item);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<List<PasswordItem>> GetAllPasswordItemsAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                return await _database.Table<PasswordItem>()
                                .Where(p => p.UserId == userId)
                                .ToListAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<int> DeletePasswordItemAsync(PasswordItem item)
        {
            try
            {
                return await _database.DeleteAsync(item);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
