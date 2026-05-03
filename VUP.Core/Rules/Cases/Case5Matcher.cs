using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case5Matcher : BaseMatcher
    {
        public override int Priority => 50;
        public override int CaseType => 5;

        public override bool IsMatch(WordNode node)
        {
            bool hasObj = node.HasChild("obj") || node.HasChild("dobj");

            bool hasPrtOrAdv = node.HasChild("prt") || node.HasChild("advmod");

            return hasObj && hasPrtOrAdv;
        }

        protected override string ExtractAction(WordNode node)
        {
            var prtNode = node.FindChild("prt") ?? node.FindChild("advmod");

            return $"{node.Lemma} {prtNode?.Lemma}".Trim().ToLower();
        }

        protected override string ExtractObject(WordNode node)
        {
            var objNode = node.FindChild("obj") ?? node.FindChild("dobj");
            return objNode?.Text ?? "";
        }
    }
}