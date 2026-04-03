using VUP.Core.Models;

namespace VUP.Core.Rules
{
    internal interface IVpcMatcher
    {
        int CaseType { get; }
        bool IsMatch(WordNode root);
        ExtractionResult Extract(WordNode root);
    }
}
