using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    // Case 9: Verb + Obj + To-Infinitive - vd: "allow us to use"
    public class Case9Matcher : BaseMatcher
    {
        public override int CaseType => 9;
        public override int Priority => 90;

        public override bool IsMatch(WordNode root)
        {
            var xcomp = root.FindChild("xcomp");
            return root.HasChild("dobj") && xcomp != null && xcomp.FindChild("mark")?.Lemma == "to";
        }

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} s.o to do sth";
        protected override string ExtractObject(WordNode root) => root.FindChild("dobj")?.Text ?? "";
    }
}
