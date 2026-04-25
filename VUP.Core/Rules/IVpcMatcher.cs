using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public interface IVpcMatcher
    {
        int CaseType { get; }
        int Priority { get; } // Thêm trường này
        bool IsMatch(WordNode root);
        ExtractionResult Extract(WordNode root);
    }
}