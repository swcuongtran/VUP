using Microsoft.Extensions.DependencyInjection;
using VUP.Core.Entities;
using VUP.Core.Models;
using VUP.Core.Rules;
using VUP.Core.Rules.Cases;

namespace VUP.Core.Engine
{
    public class VupProcessor
    {
        private readonly List<IVpcMatcher> _matchers;
        private HashSet<string> _verbDictionary = new();
        private readonly IServiceScopeFactory _scopeFactory;

        // Tiêm IServiceScopeFactory để Singleton có thể gọi DbContext (vốn là Scoped)
        public VupProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            // 1. Nạp 15 Luật Cú pháp
            var rawMatchers = new List<BaseMatcher>
            {
                new Case1Matcher(), new Case2Matcher(), new Case3Matcher(),
                new Case4Matcher(), new Case5Matcher(), new Case6Matcher(),
                new Case7Matcher(), new Case8Matcher(), new Case9Matcher(),
                new Case10Matcher(), new Case11Matcher(), new Case12Matcher(),
                new Case13Matcher(), new Case14Matcher(), new Case15Matcher()
            };

            // Tự động sắp xếp mức độ ưu tiên từ khó xuống dễ
            _matchers = rawMatchers.OrderByDescending(m => m.Priority).Cast<IVpcMatcher>().ToList();

            // 2. Nạp Từ điển TỪ DATABASE thật lên RAM
            LoadDictionaryFromDatabase();
        }

        private void LoadDictionaryFromDatabase()
        {
            // Tạo một scope ngắn hạn để mượn VupDbContext
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VupDbContext>();

            // Select toàn bộ động từ nạp vào HashSet (Tốc độ O(1))
            var allVerbs = db.Verbs.Select(v => v.Lemma.ToLower()).ToList();
            _verbDictionary = new HashSet<string>(allVerbs);

            Console.WriteLine($"[VUP Engine] Đã nạp {_verbDictionary.Count} động từ từ PostgreSQL lên RAM.");
        }

        public ExtractionResult? Process(WordNode root)
        {
            foreach (var matcher in _matchers)
            {
                if (matcher.IsMatch(root))
                {
                    var result = matcher.Extract(root);

                    // Tra cứu xem động từ này có trong DB không?
                    bool existsInDb = _verbDictionary.Contains(root.Lemma.ToLower());

                    // NẾU LÀ TỪ MỚI (Ví dụ: "rizz up") -> Ghi vào Database
                    if (!existsInDb)
                    {
                        // Gọi hàm lưu bất đồng bộ (Fire and forget) để không làm chậm API của User
                        _ = RecordUnknownVerbAsync(result.Action, result.Type);
                    }

                    return result with { IsFromDictionary = existsInDb };
                }
            }
            return null;
        }

        // Hàm chạy ngầm (Background Task) để lưu từ mới
        private async Task RecordUnknownVerbAsync(string rawAction, int type)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VupDbContext>();

                // Kiểm tra xem từ này đã từng bị report trước đây chưa
                var existing = db.UnknownVerbs.FirstOrDefault(u => u.RawAction == rawAction && u.DetectedType == type);

                if (existing != null)
                {
                    existing.Frequency += 1;
                    existing.LastSeenAt = DateTime.UtcNow;
                }
                else
                {
                    db.UnknownVerbs.Add(new UnknownVerb
                    {
                        RawAction = rawAction,
                        DetectedType = type,
                        Status = "Pending"
                    });
                }

                await db.SaveChangesAsync();
                Console.WriteLine($"[Learning] Đã phát hiện và ghi nhận từ mới: {rawAction} (Type {type})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Không thể lưu từ mới: {ex.Message}");
            }
        }
    }
}