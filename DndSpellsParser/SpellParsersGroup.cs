using Shared;

namespace DndSpellsParser;

/// <summary>
/// Управляет группой парсеров заклинаний
/// </summary>
public class SpellParsersGroup : IDisposable
{
    public double[] UnitsProgress => _parsers.Select(x => x.Progress).ToArray();

    private readonly ISpellParser[] _parsers;

    public SpellParsersGroup(params ISpellParser[] parsers) => _parsers = parsers;

    public async Task<IEnumerable<Spell>> RunAsync()
    {
        var tasks = _parsers.Select(x => x.ParseSpellsAsync()).ToArray();

        await Task.WhenAll(tasks);

        return tasks.SelectMany(x => x.Result);
    }

    public void Dispose()
    {
        foreach (var p in _parsers)
        {
            try
            {
                p.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine($"err >> {e.Message}");
            }
        }
    }
}