using SQLite;

namespace SecureVault.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
