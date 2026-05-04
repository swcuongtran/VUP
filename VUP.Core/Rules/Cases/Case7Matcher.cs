using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    // Case 7: Di-Transitive Verb (2 tân ngữ) - vd: "give him a book"
    public class Case7Matcher : BaseMatcher
    {
        public override int CaseType => 7;
        public override int Priority => 120;

        public override bool IsMatch(WordNode root) =>
            root.HasChild("dobj") && root.HasChild("iobj");

        protected override string ExtractAction(WordNode root) => root.Lemma;

        protected override string ExtractObject(WordNode root)
        {
            var iobj = root.FindChild("iobj")?.Text;
            var dobj = root.FindChild("dobj")?.Text;
            return $"{iobj} {dobj}".Trim(); 
        }
    }
}
