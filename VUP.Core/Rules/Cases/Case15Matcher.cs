using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case15Matcher : BaseMatcher
    {
        public override int CaseType => 15;

        // CỰC KỲ QUAN TRỌNG: Priority phải cao nhất (150)
        // Phải quét Case 3 từ này trước, nếu không Case 4 hoặc 5 sẽ "giành" mất
        public override int Priority => 150;

        // Khớp khi động từ có CẢ tiểu từ (prt) VÀ cụm giới từ (nmod)
        public override bool IsMatch(WordNode root) =>
            root.HasChild("compound:prt") && root.HasChild("nmod");

        protected override string ExtractAction(WordNode root)
        {
            // Lấy tiểu từ (Ví dụ: "up" trong "put up with")
            var prt = root.FindChild("compound:prt")?.Lemma ?? "";

            // Tìm cụm tân ngữ chứa giới từ
            var nmod = root.FindChild("nmod");

            // Lấy giới từ của cụm đó (Ví dụ: "with" trong cụm "with the noise")
            var prep = nmod?.FindChild("case")?.Lemma ?? "";

            // Ghép 3 từ lại: "put" + "up" + "with"
            return $"{root.Lemma} {prt} {prep}".Trim();
        }

        protected override string ExtractObject(WordNode root)
        {
            // Tân ngữ chính là danh từ bị giới từ chi phối ("noise", "party")
            return root.FindChild("nmod")?.Lemma ?? "Unknown";
        }
    }
}