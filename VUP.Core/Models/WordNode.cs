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
        public WordNode? FindChild(string dep)
        {
            if (dep == "dobj")
                return Children.FirstOrDefault(c => c.Dep == "dobj" || c.Dep == "obj");

            return Children.FirstOrDefault(child => child.Dep == dep || child.Dep.Contains(dep));
        }

        public bool HasChild(string dep)
        {
            if (dep == "dobj")
                return Children.Any(c => c.Dep == "dobj" || c.Dep == "obj");

            return Children.Any(child => child.Dep == dep || child.Dep.Contains(dep));
        }
    }
}