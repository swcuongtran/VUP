using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case4Matcher : BaseMatcher
    {
        public override int CaseType => 4;
        public override int Priority => 40;

        // Khớp khi có nmod (giới từ đi kèm danh từ) hoặc prt
        public override bool IsMatch(WordNode root) =>
            (root.HasChild("dobj") && root.HasChild("compound:prt")) || root.HasChild("nmod");

        protected override string ExtractAction(WordNode root)
        {
            var prt = root.FindChild("compound:prt")?.Lemma ?? root.FindChild("nmod")?.Lemma; // Lấy giới từ của nmod
            return $"{root.Lemma} {prt}".Trim();
        }
        protected override string ExtractObject(WordNode root) =>
            root.FindChild("dobj")?.Lemma ?? root.FindChild("nmod")?.Text ?? "Unknown";
    }
}
