using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public class Case5Matcher : IVpcMatcher
    {
        public int CaseType => 5;

        public ExtractionResult Extract(WordNode root)
        {
            var subj = root.FindChild("nsubj")?.Text ?? "Unknown";
            var obj = root.FindChild("dobj")?.Text ?? "Unknown";
            var prt = root.FindChild("compound:prt")?.Text ?? "";

            return new ExtractionResult
            (
                Subject : subj,
                Action : $"{root.Text} {prt}".Trim(),
                Object : obj,
                Type : CaseType,
                IsFromDictionary : false
            );
        }

        public bool IsMatch(WordNode root) => root.HasChild("dobj") && root.HasChild("compound:prt");


    }
}
