using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case13Matcher : BaseMatcher
    {
        public override int CaseType => 13;
        public override int Priority => 130;

        public override bool IsMatch(WordNode root) => root.HasChild("ccomp"); 

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} that";
        protected override string ExtractObject(WordNode root) => root.FindChild("ccomp")?.Lemma ?? "";
    }
}
