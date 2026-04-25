using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case15Matcher : BaseMatcher
    {
        public override int CaseType => 15;

        public override int Priority => 150;

        public override bool IsMatch(WordNode root) =>
            root.HasChild("compound:prt") && root.HasChild("nmod");

        protected override string ExtractAction(WordNode root)
        {
            var prt = root.FindChild("compound:prt")?.Lemma ?? "";

            var nmod = root.FindChild("nmod");

            var prep = nmod?.FindChild("case")?.Lemma ?? "";

            return $"{root.Lemma} {prt} {prep}".Trim();
        }

        protected override string ExtractObject(WordNode root)
        {
            return root.FindChild("nmod")?.Lemma ?? "Unknown";
        }
    }
}