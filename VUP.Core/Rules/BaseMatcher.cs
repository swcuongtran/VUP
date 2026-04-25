using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public abstract class BaseMatcher : IVpcMatcher
    {
        public abstract int CaseType { get; }
        public abstract int Priority { get; }

        public abstract bool IsMatch(WordNode root);

        protected abstract string ExtractAction(WordNode root);
        protected abstract string ExtractObject(WordNode root);

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
                IsFromDictionary: true 
            );
        }

        protected string FindSubject(WordNode root)
        {
            var nsubj = root.FindChild("nsubj");
            if (nsubj != null) return nsubj.Text;

            var nsubjpass = root.FindChild("nsubjpass");
            if (nsubjpass != null) return nsubjpass.Text;

            var xsubj = root.FindChild("xsubj");
            if (xsubj != null) return xsubj.Text;

            var csubj = root.FindChild("csubj");
            if (csubj != null) return "[Mệnh đề phụ]";

            return "Unknown";
        }
    }
}