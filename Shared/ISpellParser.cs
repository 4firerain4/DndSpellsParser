using ISpellParser;

namespace Shared
{
    public interface ISpellParser
    {
        double Progress { get; }
        Task<IEnumerable<Spell>> ParseSpellsAsync();
    }
}