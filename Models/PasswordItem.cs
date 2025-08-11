using SQLite;

namespace SecureVault.Models
{
    public class PasswordItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string Notes { get; set; }
        public string GroupTitle { get; set; }
        public int UserId { get; set; }
    }
}
