using System.Text.Json.Nodes;
using Shared;

namespace TTGClub
{
    public class SpellParser : ISpellParser
    {
        public double Progress => _linksProcessed / (double)_totalLinks;
        private int _totalLinks = 1;
        private int _linksProcessed = 0;
        private readonly string _spellListUrl = "https://ttg.club/api/v1/spells";
        private readonly HttpClient _client = new();
        private List<Spell> _spells = new();

        public async Task<IEnumerable<Spell>> ParseSpellsAsync()
        {
            var requestBody = """{"page":0,"size":999999999,"search":{"value":"","exact":false},"order":[{"field":"level","direction":"asc"},{"field":"name","direction":"asc"}]}""";

            using var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            var linksInJson = await SendRequestAsync(content, _spellListUrl);
            var spellLinks = ParseLinks(linksInJson);
            _totalLinks = spellLinks.Count;

            int iterator = 0;

            foreach (string link in spellLinks)
            {
                await Task.Delay(50);
                _spells.Add(SpellBuilder.FromJson(await SendRequestAsync(content, link)));
                
                iterator++;
                _linksProcessed++;
            }

            return _spells;
        }
        private async Task<string> SendRequestAsync(StringContent content, string link)
        {
            var response = await _client.PostAsync(link, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        
        private List<string> ParseLinks(string jsonLinks)
        {
            JsonArray array = JsonNode.Parse(jsonLinks).AsArray();
            return array.Select(x => $"https://ttg.club/api/v1/spells{x["url"].ToString().Substring(7)}").ToList();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}