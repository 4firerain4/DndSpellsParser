#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace ISpellParser;

public class Spell
{
    public string Title;
    public string TitleEn;
    public string Level;
    public string School;
    public string Url;
    public string Distance;
    public string CastingTime;
    public string Duration;
    public string Components;
    public string Description;
    public string[] UnitClasses;
    public string[] Sources;
}