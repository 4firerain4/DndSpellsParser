using Shared;
using TTGClubParser;

namespace DndSpellsParser;

class Program
{
    static async Task Main(string[] args)
    {
        string url = args[0]; 
        ParserManager manager = new();
        await manager.ParseSpellsAsync(url);

        Console.WriteLine("Done");
    }
}