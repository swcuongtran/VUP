using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUP.Core.Models;

namespace VUP.Core.Rules.Cases
{
    public class Case12Matcher : BaseMatcher
    {
        public override int CaseType => 12;
        public override int Priority => 120; // Độ ưu tiên cao hơn 11 để hứng trước

        // Y hệt code Java: Thêm điều kiện KHÔNG CÓ xcomp
        public override bool IsMatch(WordNode root) =>
            !root.HasChild("dobj") && !root.HasChild("iobj") && !root.HasChild("xcomp") && root.HasChild("advcl");

        protected override string ExtractAction(WordNode root) => $"{root.Lemma} prep s.o doing sth";

        protected override string ExtractObject(WordNode root) => root.FindChild("advcl")?.Lemma ?? "";
    }
}
