namespace DnDSu;

public class SpellParser
{
    public static async Task<IEnumerable<Spell>> Parse()
    {
        using var handbook = new SeleniumSpellLinksParser();
        using var homebrew = new SeleniumSpellLinksParser();

        var links = await handbook.Parse("https://dnd.su/spells/");
        links = links.Concat(await homebrew.Parse("https://dnd.su/homebrew/spells/"));

        using HttpClient client = new();

        return await TryParse(links, client);
    }

    private static async Task<IEnumerable<Spell>> TryParse(IEnumerable<string> links, HttpClient client)
    {
        const int maxRetries = 10;
        int totalLinks = links.Count();
        IEnumerable<Spell> parsedSpells = new List<Spell>();

        for (int i = 0; i < maxRetries; i++)
        {
            var spellConstructors = links.Select(x =>
            {
                Thread.Sleep(200); // Сайт начал блочить без этой задержки
                return new SpellDataParser(client, x).ConstructSpell();
            }).ToArray();
            
            await Task.WhenAll(spellConstructors);
            parsedSpells = parsedSpells.Concat(spellConstructors.Where(x => x.Result.isSuccess).Select(x => x.Result.spell));

            links = spellConstructors.Where(x => !x.Result.isSuccess).Select(x => x.Result.spell.Url);
            if (!links.Any()) break;
        }

        if (links.Any())
        {
            Console.WriteLine($"dnd.su parser >> {links.Count()} из {totalLinks} ссылок не удалось спарсить.");
        }

        return parsedSpells;
    }
}