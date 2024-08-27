using HtmlAgilityPack;

namespace DnDSu;

internal class SpellDataParser
{
    private readonly HtmlDocument _document;
    private readonly string _url;
    private readonly HttpClient _client;

    public SpellDataParser(HttpClient client, string url)
    {
        _document = new();
        _url = url;
        _client = client;
    }

    public async Task<(Spell spell, bool isSuccess)> ConstructSpell()
    {
        Spell spell = new();
        spell.Url = _url;

        try
        {
            var html = await LoadPage(_url);
            _document.LoadHtml(html);

            (spell.Title, spell.TitleEn) = PullTitles();
            (spell.Level, spell.School) = PullLevelLine();
            spell.CastingTime = PullStrongList(2);
            spell.Distance = PullStrongList(3);
            spell.Components = PullStrongList(4);
            spell.Duration = PullStrongList(5);
            spell.UnitClasses = PullStrongList(6).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            spell.Sources = PullSources();
            spell.Description = PullDescription();
        }
        catch
        {
#if DEBUG
            Console.WriteLine($"dnd.su parser >> Ошибка сборки заклинания: {spell.Url}.");
#endif
            return (spell, false);
        }

        return (spell, true);
    }

    private string[] PullSources()
        => PullStrongList(7).Replace("«<span>", "").Replace("</span>»", "").Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    private (string title, string titleEn) PullTitles()
    {
        var titles = _document.DocumentNode.SelectSingleNode("//h2[@class='card-title']/span").InnerText
            .Split("[]".ToCharArray(), StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return (titles[0], titles.Length > 1 ? titles[1] : string.Empty);
    }

    private string PullDescription()
    {
        var description = _document.DocumentNode.SelectSingleNode("//div[@itemprop='description']").InnerText.Replace("&nbsp;", " ");
        return description;
    }

    private (string level, string school) PullLevelLine()
    {
        var levelLine = _document.DocumentNode.SelectSingleNode("(//ul[contains(@class,'card__article-body')]/li)[1]").InnerText;
        var commaPos = levelLine.IndexOf(',');
        var level = levelLine[0..commaPos];

        string school = string.Empty;
        if (commaPos != -1)
            school = levelLine[(commaPos + 1)..].Trim();

        return (level, school);
    }

    private string PullStrongList(int line)
    {
        var data = _document.DocumentNode.SelectSingleNode($"(//ul[contains(@class,'card__article-body')]/li)[{line}]").InnerHtml
            .Split("</strong>", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[^1];
        return data;
    }

    private async Task<string> LoadPage(string url)
    {
        string result = string.Empty;
        using HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        throw new Exception("Problems with loading page");
    }
}