using VUP.Core.Models;

namespace VUP.Core.Rules
{
    public interface IVpcMatcher
    {
        int CaseType { get; }
        int Priority { get; } 
        bool IsMatch(WordNode root);
        ExtractionResult Extract(WordNode root);
    }
}