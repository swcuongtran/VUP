using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case10Matcher : BaseMatcher
    {
        public override int CaseType => 10;
        public override int Priority => 170;

        public override bool IsMatch(WordNode root)
        {
            var xcomp = root.FindChild("xcomp");
            return xcomp != null && xcomp.Pos.StartsWith("VBG"); // Bắt các dạng V-ing
        }

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} doing sth";

        protected override string ExtractObject(WordNode root)
        {
            var xcomp = root.FindChild("xcomp");
            if (xcomp == null) return "Unknown";

            // Nếu xcomp (V-ing) có tân ngữ bên trong (như "watching" có "sunset"), thì lấy toàn bộ cụm
            var innerObj = xcomp.FindChild("obj") ?? xcomp.FindChild("dobj");
            if (innerObj != null)
            {
                return $"{xcomp.Lemma} {innerObj.Text}".Trim();
            }

            return xcomp.Lemma;
        }
    }
}