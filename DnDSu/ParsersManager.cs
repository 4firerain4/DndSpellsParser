namespace DnDSu;

public class ParsersManager : IDisposable
{
    public double ProgressStatus => 1 - (double)_linksLeft / _totalLinks;

    private int _totalLinks = 1;
    private int _linksLeft = 1;
    private string[] _links = null!;
    private readonly HttpClient _httpClient = new();

    public async Task<IEnumerable<Spell>> Parse()
    {
        using var linksParser = new SeleniumSpellLinksParser();
        
        _links = (await linksParser.GetLinksFromUrl("https://dnd.su/spells/")).Concat(await linksParser.GetLinksFromUrl("https://dnd.su/homebrew/spells/")).ToArray();
        
        _totalLinks = _links.Length;
        _linksLeft = _totalLinks;

        return await TryParseSpellsData();
    }

    private async Task<IEnumerable<Spell>> TryParseSpellsData()
    {
        const int maxRetries = 10;
        IEnumerable<Spell> parsedSpells = new List<Spell>();

        for (int i = 0; i < maxRetries; i++)
        {
            var spellConstructors = _links.Select(x =>
            {
                Thread.Sleep(200); // Сайт начал блочить без этой задержки
                return new SpellDataParser(_httpClient, x).ConstructSpell();
            }).ToArray();

            await Task.WhenAll(spellConstructors);
            parsedSpells = parsedSpells.Concat(spellConstructors.Where(x => x.Result.isSuccess).Select(x => x.Result.spell));

            _links = spellConstructors.Where(x => !x.Result.isSuccess).Select(x => x.Result.spell.Url).ToArray();
            _linksLeft = _links.Length;

            if (_linksLeft == 0) break;
        }

        if (_links.Any())
        {
            Console.WriteLine($"dnd.su parser >> {_links.Count()} из {_totalLinks} ссылок не удалось спарсить.");
        }

        return parsedSpells;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}