using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case9Matcher : BaseMatcher
    {
        public override int CaseType => 9;
        public override int Priority => 190;

        public override bool IsMatch(WordNode root)
        {
            var xcomp = root.FindChild("xcomp");
            return root.HasChild("dobj") && xcomp != null && xcomp.FindChild("mark")?.Lemma == "to";
        }

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} s.o to do sth";
        protected override string ExtractObject(WordNode root) => root.FindChild("dobj")?.Text ?? "";
    }
}
