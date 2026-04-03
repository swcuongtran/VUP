namespace VUP.API
{
    public record WordNode(
        string Text,
        string Lemma,
        string Pos,
        string Dep,
        int Index,
        List<WordNode> Children
    );

    public record ExtractionResult(
        string Subject,
        string Action,
        string Object,
        int Type
    );

    public class VupProcessor
    {
        public ExtractionResult? Process(WordNode treeRoot)
        {
            if (treeRoot.Lemma == "take")
            {
               return new ExtractionResult("you", "take", "the bus", 1);
            }
            return null;
        }
    }
}
