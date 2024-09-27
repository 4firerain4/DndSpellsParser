namespace Shared
{
    public interface ISpellParser : IDisposable
    {
        double Progress { get; }
        Task<IEnumerable<Spell>> ParseSpellsAsync(params string[] args);
    }
}