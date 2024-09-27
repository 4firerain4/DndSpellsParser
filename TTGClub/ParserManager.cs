using Shared;

namespace TTGClub
{
    public class ParserManager : ISpellParser
    {
        public double Progress => 0; // TODO: Доделать прогресс

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Spell>> ParseSpellsAsync()
        {
            string url = ""; // TODO: Зачем ты принимаешь массив ссылок? Твой парсер должен сам внутренней логикой их получить с сайта и вернуть уже спаршенные заклинания 
            List<string> jsonLinks = await Parser.PostRequestAsync(url);

            List<string> clearLinks = JsongSpellParser.ParseLinks(jsonLinks);

            clearLinks = clearLinks.Select(p => url + p).ToList();

            List<string> jsonSpells = await Parser.PostRequestAsync(clearLinks.ToArray());

            List<Spell> spells = JsongSpellParser.ParseInfo(jsonSpells);
            return spells;
        }

    }
}