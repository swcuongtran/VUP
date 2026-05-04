using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case6Matcher : BaseMatcher
    {
        public override int CaseType => 6;
        public override int Priority => 110; // Đảm bảo Priority cao hơn Case 3 (70)

        public override bool IsMatch(WordNode root)
        {
            // Kiểm tra có tân ngữ trực tiếp VÀ có cụm giới từ (nmod hoặc obl)
            bool hasDirectObj = root.HasChild("dobj") || root.HasChild("obj");
            bool hasPrepPhrase = root.HasChild("nmod") || root.HasChild("obl");

            return hasDirectObj && hasPrepPhrase;
        }

        protected override string ExtractAction(WordNode root)
        {
            // Tìm cụm giới từ
            var prepNode = root.FindChild("nmod") ?? root.FindChild("obl");

            // Lấy giới từ (nằm ở nhãn case)
            var caseNode = prepNode?.FindChild("case")?.Lemma;

            // Nối Action và Giới từ
            var parts = new[] { root.Lemma, caseNode }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts).ToLower();
        }

        protected override string ExtractObject(WordNode root)
        {
            // Tùy nhu cầu bạn muốn lấy "him" (Text) hay "he" (Lemma)
            // Lấy Text sẽ giữ được nguyên trạng câu gốc
            var objNode = root.FindChild("dobj") ?? root.FindChild("obj");
            return objNode?.Text ?? "Unknown";
        }
    }
}