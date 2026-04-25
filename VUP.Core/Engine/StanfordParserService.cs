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
        private readonly Morphology _morphology; 

        public StanfordParserService(string modelsPath)
        {
            _tagger = new MaxentTagger(Path.Combine(modelsPath, "english-left3words-distsim.tagger"));
            _parser = LexicalizedParser.loadModel(Path.Combine(modelsPath, "englishPCFG.ser.gz"));
            _morphology = new Morphology();
        }

        public WordNode? ParseToTree(string text)
        {
            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var rawWords = tokenizerFactory.getTokenizer(new java.io.StringReader(text)).tokenize();
            var tagged = _tagger.tagSentence(rawWords);

            var parsedTree = (Tree)_parser.apply(tagged);

            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(parsedTree);
            var tdl = gs.typedDependenciesCollapsed(); 

            return BuildWordNodeTree(tdl.toArray());
        }

        private WordNode? BuildWordNodeTree(object[] dependencies)
        {
            var nodeDict = new Dictionary<int, WordNode>();
            WordNode? rootNode = null;

            foreach (TypedDependency tdep in dependencies)
            {
                int govIdx = tdep.gov().index(); // Node cha
                int depIdx = tdep.dep().index(); // Node con

                if (!nodeDict.ContainsKey(depIdx) && depIdx > 0)
                {
                    string word = tdep.dep().value();
                    string pos = tdep.dep().tag();
                    string lemma = _morphology.lemma(word, pos);

                    nodeDict[depIdx] = new WordNode(word, lemma, pos, tdep.reln().getShortName(), depIdx, new List<WordNode>());
                }

                if (govIdx == 0)
                {
                    rootNode = nodeDict[depIdx];
                    rootNode = rootNode with { Dep = "ROOT" }; 
                    nodeDict[depIdx] = rootNode;
                }
            }

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