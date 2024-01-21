using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DndSpellsParser.DndSuServices;

public class SpellsParser
{
    private static HttpClient _client = new();
    private ILogger _logger;

    public SpellsParser()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger("SpellsParser dnd.su");
    }

    public async Task<IEnumerable<string>> GetSpellsLinks()
    {
        var response = await _client.GetAsync("https://dnd.su/spells/");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogCritical("Failed to get spells links list");
            return [];
        }

        var doc = new HtmlAgilityPack.HtmlDocument();
        var html = await response.Content.ReadAsStringAsync();
        doc.LoadHtml(html);

        var a = doc.DocumentNode
            .SelectNodes("//a[@class=\"cards_list__item-wrapper\"]");

        return a.Select(link => link.Attributes["href"].Value);
    }
}