using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public abstract class BaseMatcher : IVpcMatcher
    {
        // Thuộc tính để C# tự động sắp xếp độ ưu tiên (Giống dòng 612-632 code Java)
        public abstract int CaseType { get; }
        public abstract int Priority { get; }

        // Điều kiện để khớp luật
        public abstract bool IsMatch(WordNode root);

        // Các thành phần đặc thù do từng Case tự bóc tách
        protected abstract string ExtractAction(WordNode root);
        protected abstract string ExtractObject(WordNode root);

        // Template Method: Gánh toàn bộ phần lặp lại của 14 hàm ProcessVerbType trong Java
        public ExtractionResult Extract(WordNode root)
        {
            string subject = FindSubject(root);
            string action = ExtractAction(root);
            string obj = ExtractObject(root);

            return new ExtractionResult
            (
                Subject: subject,
                Action: action,
                Object: obj,
                Type: CaseType,
                IsFromDictionary: true // Sẽ được Processor ghi đè
            );
        }

        // Sao chép logic hàm FindSubjIdx (Dòng 1146 - VUPMatcher.java)
        protected string FindSubject(WordNode root)
        {
            // Ưu tiên 1: Chủ ngữ chủ động (nsubj)
            var nsubj = root.FindChild("nsubj");
            if (nsubj != null) return nsubj.Text;

            // Ưu tiên 2: Chủ ngữ bị động (nsubjpass)
            var nsubjpass = root.FindChild("nsubjpass");
            if (nsubjpass != null) return nsubjpass.Text;

            // Ưu tiên 3: Chủ ngữ kiểm soát (xsubj) - Dùng trong To-infinitive
            var xsubj = root.FindChild("xsubj");
            if (xsubj != null) return xsubj.Text;

            // Ưu tiên 4: Mệnh đề làm chủ ngữ (csubj)
            var csubj = root.FindChild("csubj");
            if (csubj != null) return "[Mệnh đề phụ]";

            return "Unknown";
        }
    }
}