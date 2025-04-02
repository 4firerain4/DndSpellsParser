using HtmlAgilityPack;
using Shared;

namespace DnDSu;

/// <summary>
/// Конструирует конкретное заклинание
/// </summary>
internal class SpellConstructor
{
    private readonly HtmlDocument _document;
    private readonly string _url;
    private readonly HttpClient _client;

    public SpellConstructor(HttpClient client, string url)
    {
        _document = new();
        _url = url;
        _client = client;
    }

    public async Task<Spell> ConstructSpell()
    {
        Spell spell = new()
        {
            Url = _url
        };

        try
        {
            var html = await LoadPage(_url);
            _document.LoadHtml(html);

            (spell.Title, spell.TitleEn) = PullTitles();
            (spell.Level, spell.School) = PullLevelLine();
            spell.CastingTime = PullStrongList(2);
            spell.Distance = PullStrongList(3);
            spell.Duration = PullStrongList(5);
            spell.Sources = PullSources();
            spell.Description = PullDescription();
            (spell.ComponentS, spell.ComponentV, spell.ComponentM) = PullComponents();
            spell.UnitClasses = PullStrongList(6).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }
        catch
        {
            // #if DEBUG
            //     Console.WriteLine($"dnd.su parser >> Ошибка сборки заклинания: {spell.Url}.");
            // #endif
            throw new Exception($"Ошибка сборки заклинания: {spell.Url}");
        }

        return spell;
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

    private (bool S, bool V, string M) PullComponents()
    {
        const int lengthOfComponentPart = 9;
        bool s, v, m = false;
        string materials = string.Empty;

        string componentLine = PullStrongList(4);
        int indexOfMaterials = componentLine.IndexOf("М (", StringComparison.Ordinal);
        if (indexOfMaterials != -1) m = true;
        int rangePos = Math.Min(lengthOfComponentPart, m ? indexOfMaterials : componentLine.Length);

        s = componentLine[..rangePos].Contains("С");
        v = componentLine[..rangePos].Contains("В");

        if (m) materials = componentLine[(indexOfMaterials + 3)..^1]; // 3 - длина "М ("

        return (s, v, materials);
    }

    private string PullStrongList(int line)
    {
        if (line == 6)
            if (!_document.DocumentNode.SelectSingleNode($"(//ul[contains(@class,'card__article-body')]/li)[{line}]").InnerHtml.ToString().Contains("<strong>Классы:"))
                return "";
    
        if (line == 7)
            if (!_document.DocumentNode.SelectSingleNode($"(//ul[contains(@class,'card__article-body')]/li)[{line}]").InnerHtml.ToString().Contains("<strong>Источник:"))
                line--;
            
        var data = _document.DocumentNode.SelectSingleNode($"(//ul[contains(@class,'card__article-body')]/li)[{line}]").InnerHtml
            .Split("</strong>", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[^1];
        return data;
    }

    private async Task<string> LoadPage(string url)
    {
        using HttpResponseMessage response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        throw new Exception("Problems with loading page");
    }
}