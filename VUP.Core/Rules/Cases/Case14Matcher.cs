using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    // Case 14: Linking verb - vd: "seems tired"
    public class Case14Matcher : BaseMatcher
    {
        public override int CaseType => 14;
        public override int Priority => 65;

        public override bool IsMatch(WordNode root) =>
            !root.HasChild("dobj") && root.HasChild("xcomp");

        protected override string ExtractAction(WordNode root) => root.Lemma;
        protected override string ExtractObject(WordNode root) => root.FindChild("xcomp")?.Lemma ?? "";
    }
}
