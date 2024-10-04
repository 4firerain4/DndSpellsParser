using Shared;

namespace DnDSu;

public class SpellParser : ISpellParser
{
    public double Progress => _linksProcessed / (double)_totalLinks; // TODO: Расчёт с учётом Selenium

    private int _totalLinks = 1;
    private int _linksProcessed = 0;
    private string[] _links = null!;
    private readonly HttpClient _httpClient = new();

    public async Task<IEnumerable<Spell>> ParseSpellsAsync()
    {
        using var homebrewParser = new SeleniumSpellLinksParser();
        using var handbookParser = new SeleniumSpellLinksParser();

        _links = (await handbookParser.GetLinksFromUrl("https://dnd.su/spells/")).Concat(await homebrewParser.GetLinksFromUrl("https://dnd.su/homebrew/spells/")).ToArray();

        _totalLinks = _links.Length;

        return await TryParseSpellsData();
    }

    private async Task<IEnumerable<Spell>> TryParseSpellsData()
    {
        const int maxRetries = 10;
        IEnumerable<Spell> parsedSpells = new List<Spell>();

        for (int i = 0; i < maxRetries; i++)
        {
            var spellConstructors = _links.Select(x => new SpellConstructor(_httpClient, x).ConstructSpell());

            var filteredSpells = await FilterSuccessParsedSpells(spellConstructors);

            parsedSpells = filteredSpells.Sucess;
            _links = filteredSpells.Failed.ToArray();

            _linksProcessed = _totalLinks - _links.Length; // Уточнение подсчёта ссылок.

            if (_links.Length == 0) break;
        }

        if (_links.Any())
        {
            Console.WriteLine($"dnd.su parser >> {_links.Length} из {_totalLinks} ссылок не удалось спарсить.");
        }

        return parsedSpells;
    }

    private async Task<(List<Spell> Sucess, List<string> Failed)> FilterSuccessParsedSpells(IEnumerable<Task<Spell>> spellConstructors)
    {
        List<Spell> success = new();
        List<string> failed = new();

        foreach (var i in spellConstructors)
        {
            try
            {
                var spell = await i;

                success.Add(spell);
                _linksProcessed++;
            }
            catch (Exception e)
            {
                failed.Add(e.Message.Split(' ')[^1]);
            }
        }

        return (success, failed);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}