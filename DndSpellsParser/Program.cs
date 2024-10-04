namespace DndSpellsParser;

class Program
{
    static async Task Main(string[] args)
    {
        var start = DateTime.Now;
        using SpellParsersGroup group = new(new DnDSu.SpellParser(), new TTGClub.ParserManager());
        var spellsTask = group.RunAsync();

        while (group.UnitsProgress.All(x => x < 1))
        {
            await Task.Delay(1000);
            Console.Clear();

            foreach (var p in group.UnitsProgress)
            {
                const int pointsCount = 20;
                string progressBar = $"[{string.Concat(Enumerable.Range(1, pointsCount).Select(x => x <= p * pointsCount ? '=' : ' '))}] {p * 100:F}%";

                Console.WriteLine(progressBar);
            }
        }

        var spells = await spellsTask;

        Console.WriteLine($"Спаршено: {spells.Count()} заклинаний.");
        Console.WriteLine((DateTime.Now - start).TotalSeconds + "сек.");
        Console.ReadKey();
    }
}
