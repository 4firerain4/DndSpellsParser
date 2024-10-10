using Shared;

namespace TTGClub
{
    public class SpellParser : ISpellParser
    {
        public double Progress => _linksProcessed / (double)_totalLinks;
        private int _totalLinks = 1;
        private int _linksProcessed = 0;
        private string _url = "https://ttg.club/api/v1/spells";
        private List<Spell> _spells = new();
        private HttpClient _client = new();

        public async Task<IEnumerable<Spell>> ParseSpellsAsync()
        {
            var requestBody = """{"page":0,"size":999999999,"search":{"value":"","exact":false},"order":[{"field":"level","direction":"asc"},{"field":"name","direction":"asc"}]}""";

            using var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            var links = await SendRequestAsync(content, _url);
            var clearLinks = JsonSpellParser.ParseLinks(links);
            _totalLinks = clearLinks.Count;

            int iterator = 0;

            foreach (string link in clearLinks)
            {
                await Task.Delay(50);
                _spells.Add(JsonSpellParser.ParseData(await SendRequestAsync(content, link)));
                
                // #if DEBUG
                //     Console.WriteLine($"Заклинание \"{_spells[iterator].Title}\" спершено");
                // #endif
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

        public void Dispose()
        {

        }
    }
}