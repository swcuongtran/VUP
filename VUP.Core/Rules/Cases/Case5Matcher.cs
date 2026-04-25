using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case5Matcher : BaseMatcher
    {
        public override int CaseType => 5;
        public override int Priority => 50; 

        public override bool IsMatch(WordNode root) =>
            root.HasChild("dobj") && root.HasChild("compound:prt");

        protected override string ExtractAction(WordNode root)
        {
            var prt = root.FindChild("compound:prt")?.Lemma ?? "";
            return $"{root.Lemma} {prt}".Trim(); // Dùng Lemma để dễ tra DB
        }

        protected override string ExtractObject(WordNode root) =>
            root.FindChild("dobj")?.Lemma ?? "Unknown";
    }
}