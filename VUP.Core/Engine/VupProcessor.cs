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

        public VupProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var rawMatchers = new List<BaseMatcher>
            {
                new Case1Matcher(), new Case2Matcher(), new Case3Matcher(),
                new Case4Matcher(), new Case5Matcher(), new Case6Matcher(),
                new Case7Matcher(), new Case8Matcher(), new Case9Matcher(),
                new Case10Matcher(), new Case11Matcher(), new Case12Matcher(),
                new Case13Matcher(), new Case14Matcher(), new Case15Matcher()
            };

            _matchers = rawMatchers.OrderByDescending(m => m.Priority).Cast<IVpcMatcher>().ToList();

            LoadDictionaryFromDatabase();
        }

        private void LoadDictionaryFromDatabase()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VupDbContext>();

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

                    bool existsInDb = _verbDictionary.Contains(root.Lemma.ToLower());

                    if (!existsInDb)
                    {
                        _ = RecordUnknownVerbAsync(result.Action, result.Type);
                    }

                    return result with { IsFromDictionary = existsInDb };
                }
            }
            return null;
        }

        private async Task RecordUnknownVerbAsync(string rawAction, int type)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VupDbContext>();

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