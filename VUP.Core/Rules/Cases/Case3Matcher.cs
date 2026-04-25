using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case3Matcher : BaseMatcher
    {
        public override int CaseType => 3;
        public override int Priority => 30; 

        public override bool IsMatch(WordNode root) =>
            root.HasChild("dobj") && !root.HasChild("iobj") && !root.HasChild("compound:prt");

        protected override string ExtractAction(WordNode root) => root.Lemma;

        protected override string ExtractObject(WordNode root) =>
            root.FindChild("dobj")?.Lemma ?? "Unknown";
    }
}