using Shared;

namespace TTGClub
{
    public class ParserManager : ISpellParser
    {
        public double Progress => 0; // TODO: Доделать прогресс

        public void Dispose()
        {
            
        }

        public async Task<IEnumerable<Spell>> ParseSpellsAsync()
        {
            string url = "https://ttg.club/api/v1/spells";
            List<string> jsonLinks = await Parser.PostRequestAsync(url);

            List<string> clearLinks = JsongSpellParser.ParseLinks(jsonLinks);

            clearLinks = clearLinks.Select(p => url + p).ToList();

            List<string> jsonSpells = await Parser.PostRequestAsync(clearLinks.ToArray());

            List<Spell> spells = JsongSpellParser.ParseInfo(jsonSpells);
            return spells;
        }

    }
}