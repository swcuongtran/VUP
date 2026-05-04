using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case10Matcher : BaseMatcher
    {
        public override int CaseType => 10;
        public override int Priority => 170;

        public override bool IsMatch(WordNode root)
        {
            var xcomp = root.FindChild("xcomp");
            return xcomp != null && xcomp.Pos == "VBG";
        }

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} doing sth";
        protected override string ExtractObject(WordNode root) => root.FindChild("xcomp")?.Lemma ?? "";
    }
}
