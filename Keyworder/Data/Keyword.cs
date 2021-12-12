namespace Keyworder.Data
{
    public record Keyword
    {
        private readonly string name = null!;
        private readonly bool isCategory;
        private readonly List<Keyword> children = null!;

        public string Name
        {
            get => this.name;
            init => this.name = value ?? throw new ArgumentNullException(nameof(value)); 
        }

        public bool IsCategory
        {
            get => this.isCategory;
            init => this.isCategory = value;
        }

        public List<Keyword> Children
        {
            get => this.children;
            init => this.children = value ?? new List<Keyword>();
        }
    }
}
