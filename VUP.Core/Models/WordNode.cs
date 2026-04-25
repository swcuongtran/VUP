namespace VUP.Core.Models
{
    public record WordNode(
        string Text,
        string Lemma,
        string Pos,
        string Dep,
        int Index,
        List<WordNode> Children
    )
    {
        public WordNode? FindChild(string dep) => Children.FirstOrDefault(child => child.Dep == dep);
        public bool HasChild(string dep) => Children.Any(child => child.Dep == dep);
    }
}
