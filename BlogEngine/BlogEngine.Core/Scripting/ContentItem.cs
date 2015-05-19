namespace BlogEngine.Core.Scripting
{
    public class ContentItem
    {
        public string Source { get; set; }
        public int Priority { get; set; }
        public bool Defer { get; set; }
        public bool AddAtBottom { get; set; }
        public bool IsFrontEndOnly { get; set; }
    }
}
