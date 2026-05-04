using System.Linq;
using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case15Matcher : BaseMatcher
    {
        public override int Priority => 150;
        public override int CaseType => 15;

        // Các trạng từ rác cần né
        private readonly string[] _ignoredAdverbs = { "really", "very", "extremely", "just", "simply", "completely", "immediately", "exactly" };

        public override bool IsMatch(WordNode node)
        {
            var prtNode = node.FindChild("compound:prt") ?? node.FindChild("prt") ??
                          node.Children.FirstOrDefault(c => c.Dep.Contains("advmod") && !_ignoredAdverbs.Contains(c.Lemma.ToLower()));

            var prepObj = node.FindChild("nmod") ?? node.FindChild("obl");
            bool hasCase = prepObj?.HasChild("case") == true;

            return prtNode != null && hasCase;
        }

        protected override string ExtractAction(WordNode node)
        {
            var prtNode = node.FindChild("compound:prt") ?? node.FindChild("prt") ??
                          node.Children.FirstOrDefault(c => c.Dep.Contains("advmod") && !_ignoredAdverbs.Contains(c.Lemma.ToLower()));

            var prepObj = node.FindChild("nmod") ?? node.FindChild("obl");
            var caseNode = prepObj?.FindChild("case");

            var parts = new[] { node.Lemma, prtNode?.Lemma, caseNode?.Lemma }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts).ToLower();
        }

        protected override string ExtractObject(WordNode node)
        {
            var prepObj = node.FindChild("nmod") ?? node.FindChild("obl");
            return prepObj?.Text ?? "";
        }
    }
}