using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        private int CalculateSimilarity(string text, string id)
        {
            var keys = _storage.GetKeys();
            return keys.Any(item => item.Substring(0, 5) == "TEXT-" && _storage.Load(item) == text) ? 1 : 0;
        }

        private static double CalculateRank(string text)
        {
            var notLettersCount = text.Count(ch => !char.IsLetter(ch));
            return (double)notLettersCount / text.Length;
        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = "SIMILARITY-" + id;
            string similarity = CalculateSimilarity(text, id).ToString();
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            _storage.Store(similarityKey, CalculateSimilarity(text, id).ToString());

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(textKey, text);

            string rankKey = "RANK-" + id;
            string rank = CalculateRank(text).ToString();
            //TODO: посчитать rank и сохранить в БД по ключу rankKey
            _storage.Store(rankKey, rank);

            

            return Redirect($"summary?id={id}");
        }
    }
}
