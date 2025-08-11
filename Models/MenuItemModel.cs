using SQLite;

namespace SecureVault.Models
{
    public class MenuItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageTypeName { get; set; }

        public int UserId { get; set; }
        [Ignore]
        public Type PageType => Type.GetType(PageTypeName);
    }
}
    