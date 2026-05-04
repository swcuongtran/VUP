using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VUP.Core.Entities;
using VUP.Core.Models;
using VUP.Core.Rules;

namespace VUP.Core.Engine
{
    public class VupProcessor
    {
        private readonly IEnumerable<BaseMatcher> _matchers;
        private readonly IServiceScopeFactory _scopeFactory;
        private HashSet<string> _verbDictionary = new();

        public VupProcessor(IEnumerable<BaseMatcher> matchers, IServiceScopeFactory scopeFactory)
        {
            _matchers = matchers.OrderByDescending(m => m.Priority);
            _scopeFactory = scopeFactory;
            LoadDictionaryFromDatabase();
        }

        private void LoadDictionaryFromDatabase()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VupDbContext>();

                // 1. Lấy các động từ gốc (VD: run, sleep)
                var baseVerbs = db.Verbs.Select(v => v.Lemma.Trim().ToLower()).ToList();

                // 2. Lấy Phrasal Verbs bằng cách nối Lemma và Particle (VD: turn off, pick up)
                var phrasalVerbs = db.Verbs
                    .Include(v => v.Patterns)
                    .SelectMany(v => v.Patterns.Select(p => $"{v.Lemma} {p.Particle}".Trim().ToLower()))
                    .ToList();

                // 3. Lấy các từ mới đã được duyệt
                var approvedNewVerbs = db.UnknownVerbs
                                         .Where(u => u.Status == "Approved")
                                         .Select(u => u.RawAction.Trim().ToLower()).ToList();

                // Gộp tất cả lại
                _verbDictionary = new HashSet<string>(baseVerbs.Concat(phrasalVerbs).Concat(approvedNewVerbs));
                Console.WriteLine($"[VUP Engine] Đã nạp {_verbDictionary.Count} cụm động từ lên RAM.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi Khởi tạo DB] {ex.Message}");
            }
        }

        private List<WordNode> FlattenTree(WordNode node)
        {
            var list = new List<WordNode>();
            if (node == null) return list;

            list.Add(node);
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    list.AddRange(FlattenTree(child));
                }
            }
            return list;
        }

        public List<ExtractionResult> ProcessAll(WordNode root)
        {
            var results = new List<ExtractionResult>();
            var allNodes = FlattenTree(root);

            foreach (var node in allNodes)
            {
                // Bỏ qua nếu không phải động từ, hoặc là động từ to-be
                if (string.IsNullOrEmpty(node.Pos) || !node.Pos.StartsWith("V") || node.Lemma == "be")
                    continue;

                if (string.IsNullOrEmpty(node.Lemma) || node.Lemma.Length <= 1)
                    continue;

                // --- LOGIC LỌC NHIỄU MỚI THÊM VÀO ĐÂY ---
                bool hasSubject = node.HasChild("nsubj") || node.HasChild("nsubjpass") || node.HasChild("nsubj:pass");

                // Nếu từ bị ép làm động từ nhưng KHÔNG có chủ ngữ và KHÔNG ở dạng nguyên thể (VB/VBP) -> Đích thị là danh từ bị nhận nhầm
                if (!hasSubject && node.Pos != "VB" && node.Pos != "VBP")
                    continue;

                foreach (var matcher in _matchers)
                {
                    if (matcher.IsMatch(node))
                    {
                        var result = matcher.Extract(node);

                        // Bỏ qua nếu Action bị nhận vơ là Danh từ
                        if (result.Action == "day" || result.Action == "london")
                            continue;

                        string cleanAction = result.Action.Trim().ToLower();
                        bool existsInDb = _verbDictionary.Contains(node.Lemma.Trim().ToLower()) ||
                                          _verbDictionary.Contains(cleanAction);

                        if (!existsInDb)
                        {
                            _ = RecordUnknownVerbAsync(cleanAction, result.Type);
                        }

                        results.Add(result with { IsFromDictionary = existsInDb });
                        break;
                    }
                }
            }

            return results;
        }

        public ExtractionResult? Process(WordNode root)
        {
            var allResults = ProcessAll(root);
            return allResults.OrderByDescending(r => r.Type).FirstOrDefault();
        }

        private async Task RecordUnknownVerbAsync(string action, int type)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VupDbContext>();

                var existing = db.UnknownVerbs.FirstOrDefault(u => u.RawAction == action);
                if (existing != null)
                {
                    existing.Frequency += 1;
                    existing.LastSeenAt = DateTime.UtcNow;
                }
                else
                {
                    db.UnknownVerbs.Add(new Entities.UnknownVerb
                    {
                        RawAction = action,
                        DetectedType = type,
                        Frequency = 1,
                        Status = "Pending",
                        FirstSeenAt = DateTime.UtcNow,
                        LastSeenAt = DateTime.UtcNow
                    });
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"[Learning] Đã cập nhật từ mới: {action} (Type {type})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi Learning] {ex.Message}");
            }
        }
    }
}