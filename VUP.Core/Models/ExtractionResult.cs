namespace VUP.Core.Models
{
    public record ExtractionResult(
        string Subject,
        string Action,
        string Object,
        int Type,
        bool IsFromDictionary = true
    );
}
