using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case6Matcher : BaseMatcher
    {
        public override int CaseType => 6;
        public override int Priority => 60;

        public override bool IsMatch(WordNode root) =>
            root.HasChild("dobj") && root.HasChild("nmod");

        protected override string ExtractAction(WordNode root)
        {
            var nmod = root.FindChild("nmod");
            var caseNode = nmod?.FindChild("case")?.Lemma ?? "";
            return $"{root.Lemma} {caseNode}".Trim();
        }
        protected override string ExtractObject(WordNode root) => root.FindChild("dobj")?.Lemma ?? "Unknown";
    }
}
