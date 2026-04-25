using VUP.Core.Models;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.process;
using edu.stanford.nlp.tagger.maxent;
using edu.stanford.nlp.trees;
using java.io;

namespace VUP.Core.Engine
{
    public class StanfordParserService
    {
        private readonly MaxentTagger _tagger;
        private readonly LexicalizedParser _parser;
        private readonly Morphology _morphology; // Công cụ xịn của Stanford để lấy nguyên thể (Lemma)

        public StanfordParserService(string modelsPath)
        {
            // Load models y hệt dòng 58, 59 code Java gốc
            _tagger = new MaxentTagger(Path.Combine(modelsPath, "english-left3words-distsim.tagger"));
            _parser = LexicalizedParser.loadModel(Path.Combine(modelsPath, "englishPCFG.ser.gz"));
            _morphology = new Morphology();
        }

        public WordNode? ParseToTree(string text)
        {
            // 1. Tokenize & Tag (Cắt từ và gán nhãn từ loại)
            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var rawWords = tokenizerFactory.getTokenizer(new java.io.StringReader(text)).tokenize();
            var tagged = _tagger.tagSentence(rawWords);

            // 2. Parse Tree (Tạo cây cú pháp)
            var parsedTree = (Tree)_parser.apply(tagged);

            // 3. Extract Dependencies (Bóc tách phụ thuộc - giống dòng 84 Java)
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(parsedTree);
            var tdl = gs.typedDependenciesCollapsed(); // Trả về List các TypedDependency

            // 4. Biến đổi List của Stanford thành Cây WordNode của chúng ta
            return BuildWordNodeTree(tdl.toArray());
        }

        private WordNode? BuildWordNodeTree(object[] dependencies)
        {
            var nodeDict = new Dictionary<int, WordNode>();
            WordNode? rootNode = null;

            // Quét vòng 1: Tạo tất cả các Node trước
            foreach (TypedDependency tdep in dependencies)
            {
                int govIdx = tdep.gov().index(); // Node cha
                int depIdx = tdep.dep().index(); // Node con

                // Khởi tạo node con nếu chưa có
                if (!nodeDict.ContainsKey(depIdx) && depIdx > 0)
                {
                    string word = tdep.dep().value();
                    string pos = tdep.dep().tag();
                    // Tự động tìm Lemma chuẩn thay vì phải code bộ Stemmer thủ công như Java cũ
                    string lemma = _morphology.lemma(word, pos);

                    nodeDict[depIdx] = new WordNode(word, lemma, pos, tdep.reln().getShortName(), depIdx, new List<WordNode>());
                }

                // Ghi nhận Root Node (Động từ trung tâm)
                if (govIdx == 0)
                {
                    rootNode = nodeDict[depIdx];
                    rootNode = rootNode with { Dep = "ROOT" }; // Ghi đè thuộc tính Dep
                    nodeDict[depIdx] = rootNode;
                }
            }

            // Quét vòng 2: Móc nối Node con vào Node cha
            foreach (TypedDependency tdep in dependencies)
            {
                int govIdx = tdep.gov().index();
                int depIdx = tdep.dep().index();

                if (govIdx > 0 && nodeDict.ContainsKey(govIdx) && nodeDict.ContainsKey(depIdx))
                {
                    nodeDict[govIdx].Children.Add(nodeDict[depIdx]);
                }
            }

            return rootNode;
        }
    }
}