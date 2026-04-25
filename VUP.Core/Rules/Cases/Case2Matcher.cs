using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case2Matcher : BaseMatcher
    {
        public override int CaseType => 2;
        public override int Priority => 20;

        public override bool IsMatch(WordNode root) =>
            !root.HasChild("dobj") && root.HasChild("compound:prt");

        protected override string ExtractAction(WordNode root)
        {
            var prt = root.FindChild("compound:prt")?.Lemma;
            return $"{root.Lemma} {prt}".Trim();
        }
        protected override string ExtractObject(WordNode root) => "";
    }
}
