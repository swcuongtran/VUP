using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case4Matcher : BaseMatcher
    {
        public override int CaseType => 4;
        public override int Priority => 90;

        public override bool IsMatch(WordNode root)
        {
            // Case 4: Phrasal verb với prt (VD: "turn off the light") 
            // HOẶC Động từ đi kèm cụm giới từ đóng vai trò tân ngữ (obl/nmod có case)
            bool hasPrt = root.HasChild("compound:prt") || root.HasChild("prt");
            var prepObj = root.FindChild("obl") ?? root.FindChild("nmod");
            bool hasCase = prepObj?.HasChild("case") == true;

            return (root.HasChild("dobj") && hasPrt) || hasCase;
        }

        protected override string ExtractAction(WordNode root)
        {
            var prt = root.FindChild("compound:prt")?.Lemma ?? root.FindChild("prt")?.Lemma;
            var prepObj = root.FindChild("obl") ?? root.FindChild("nmod");
            var caseNode = prepObj?.FindChild("case")?.Lemma;

            // Ưu tiên prt nếu có, nếu không thì lấy case của cụm giới từ
            string particle = prt ?? caseNode;

            var parts = new[] { root.Lemma, particle }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts).ToLower();
        }

        protected override string ExtractObject(WordNode root)
        {
            var prepObj = root.FindChild("obl") ?? root.FindChild("nmod");
            return root.FindChild("dobj")?.Text ?? prepObj?.Text ?? "Unknown";
        }
    }
}