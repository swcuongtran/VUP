using System.Linq;
using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case5Matcher : BaseMatcher
    {
        public override int Priority => 100;
        public override int CaseType => 5;

        private readonly string[] _ignoredAdverbs = { "really", "very", "extremely", "just", "simply", "completely", "immediately", "exactly" };

        public override bool IsMatch(WordNode node)
        {
            bool hasObj = node.HasChild("obj") || node.HasChild("dobj");

            var prtNode = node.FindChild("compound:prt") ?? node.FindChild("prt") ??
                          node.Children.FirstOrDefault(c =>
                              (c.Dep.Contains("advmod") || (c.Dep == "obl" && (c.Children == null || c.Children.Count == 0)))
                              && !_ignoredAdverbs.Contains(c.Lemma.ToLower()));

            return hasObj && prtNode != null;
        }

        protected override string ExtractAction(WordNode node)
        {
            var prtNode = node.FindChild("compound:prt") ?? node.FindChild("prt") ??
                          node.Children.FirstOrDefault(c =>
                              (c.Dep.Contains("advmod") || (c.Dep == "obl" && (c.Children == null || c.Children.Count == 0)))
                              && !_ignoredAdverbs.Contains(c.Lemma.ToLower()));

            var parts = new[] { node.Lemma, prtNode?.Lemma }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts).ToLower();
        }

        protected override string ExtractObject(WordNode node)
        {
            var objNode = node.FindChild("obj") ?? node.FindChild("dobj");
            return objNode?.Text ?? "";
        }
    }
}