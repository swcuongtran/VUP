using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case15Matcher : BaseMatcher
    {
        public override int Priority => 150;
        public override int CaseType => 15;

        public override bool IsMatch(WordNode node)
        {
            // 1. Tìm Tiểu từ (prt) HOẶC Trạng từ (advmod)
            bool hasPrtOrAdv = node.HasChild("prt") || node.HasChild("advmod");

            // 2. Tìm Tân ngữ chứa giới từ (nmod chuẩn cũ HOẶC obl chuẩn mới)
            var prepObj = node.FindChild("nmod") ?? node.FindChild("obl");

            // 3. Kiểm tra xem tân ngữ đó có chứa giới từ (case) không
            bool hasCase = prepObj?.HasChild("case") == true;

            return hasPrtOrAdv && hasCase;
        }

        // ĐÃ SỬA THÀNH PROTECTED
        protected override string ExtractAction(WordNode node)
        {
            var prtNode = node.FindChild("prt") ?? node.FindChild("advmod");
            var prepObj = node.FindChild("nmod") ?? node.FindChild("obl");
            var caseNode = prepObj?.FindChild("case");

            return $"{node.Lemma} {prtNode?.Lemma} {caseNode?.Lemma}".Trim().ToLower();
        }

        // ĐÃ SỬA THÀNH PROTECTED
        protected override string ExtractObject(WordNode node)
        {
            var prepObj = node.FindChild("nmod") ?? node.FindChild("obl");

            return prepObj?.Text ?? "";
        }
    }
}