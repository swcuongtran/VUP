using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case11Matcher : BaseMatcher
    {
        public override int CaseType => 11;
        public override int Priority => 110;

        public override bool IsMatch(WordNode root) =>
            !root.HasChild("dobj") && !root.HasChild("iobj") && root.HasChild("advcl");

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} prep doing sth";

        protected override string ExtractObject(WordNode root) => root.FindChild("advcl")?.Lemma ?? "";
    }
}
