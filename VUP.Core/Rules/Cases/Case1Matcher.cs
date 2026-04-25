using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case1Matcher : BaseMatcher
    {
        public override int CaseType => 1;
        public override int Priority => 10;

        public override bool IsMatch(WordNode root) =>
            !root.HasChild("dobj") && !root.HasChild("iobj") && !root.HasChild("compound:prt");

        protected override string ExtractAction(WordNode root) => root.Lemma;

        protected override string ExtractObject(WordNode root) => ""; // Nội động từ không có tân ngữ
    }
}