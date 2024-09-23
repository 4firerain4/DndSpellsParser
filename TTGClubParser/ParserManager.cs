using Shared;

namespace TTGClubParser
{
    public class ParserManager : Shared.ISpellParser
    {
        public double Progress => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Spell>> ParseSpellsAsync(params string[] args)
        {
            string url = args[0];
            List<string> jsonLinks = await Parser.PostRequestAsync(url);

            List<string> clearLinks = JsongSpellParser.ParseLinks(jsonLinks);

            clearLinks = clearLinks.Select(p => url + p).ToList();

            List<string> jsonSpells = await Parser.PostRequestAsync(clearLinks.ToArray());

            List<Spell> spells = JsongSpellParser.ParseInfo(jsonSpells);
            return spells;
        }

    }
}