using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public abstract class BaseMatcher
    {
        public abstract int Priority { get; }
        public abstract int CaseType { get; }

        public abstract bool IsMatch(WordNode node);

        protected abstract string ExtractAction(WordNode node);
        protected abstract string ExtractObject(WordNode node);

        // Hỗ trợ trích xuất cả chủ ngữ chủ động lẫn chủ ngữ bị động
        protected virtual string ExtractSubject(WordNode node)
        {
            var subjNode = node.FindChild("nsubj") ?? node.FindChild("nsubjpass");
            return subjNode?.Text ?? "Unknown";
        }

        public virtual ExtractionResult Extract(WordNode node)
        {
            string subject = ExtractSubject(node);
            string action = ExtractAction(node);
            string obj = ExtractObject(node);

            return new ExtractionResult(subject, action, obj, CaseType, false);
        }
    }
}