namespace SecureVault.Models
{
    public class IconOption
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public IconOption(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => Name;
    }
}
