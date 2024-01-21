using System;
using System.Threading.Tasks;

namespace DndSpellsParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dndsuSpellLinks = new DndSuServices.SpellsParser();
            foreach (var i in await dndsuSpellLinks.GetSpellsLinks())
            {
                Console.WriteLine(i);
            }
        }
    }
}
