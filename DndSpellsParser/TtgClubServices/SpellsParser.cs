using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DndSpellsParser.TtgClubServices;

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
        return [];
    }
}