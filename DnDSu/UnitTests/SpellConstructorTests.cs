using Shared;
using Xunit;

namespace DnDSu.UnitTests;

public class SpellConstructorTests
{
    private static Spell _spell = null!;

    public SpellConstructorTests()
    {
        if (_spell is not null) return;

        using var client = new HttpClient();
        _spell = new SpellConstructor(client, "https://dnd.su/homebrew/spells/8475-arcane_retribution/").ConstructSpell().Result;
    }

    [Fact]
    public void ParsingTitleRu()
    {
        Assert.Equal("Аркановое возмездие", _spell.Title);
    }

    [Fact]
    public void ParsingTitleEn()
    {
        Assert.Equal("Arcane Retribution", _spell.TitleEn);
    }

    [Fact]
    public void ParsingLevel()
    {
        Assert.Equal("2 уровень", _spell.Level);
    }

    [Fact]
    public void ParsingComponentsSVM()
    {
        var expected = (true, true, "осколок стекла");
        var actual = (_spell.ComponentS, _spell.ComponentV, _spell.ComponentM);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ParsingComponentsSV()
    {
        using var client = new HttpClient();
        var spell = await new SpellConstructor(client, "https://dnd.su/homebrew/spells/678-adaptation/").ConstructSpell();

        var expected = (true, true, String.Empty);
        var actual = (spell.ComponentS, spell.ComponentV, spell.ComponentM);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ParsingComponentsM()
    {
        using var client = new HttpClient();
        var spell = await new SpellConstructor(client, "https://dnd.su/homebrew/spells/724-velikii-krug-uderzaniia/").ConstructSpell();

        var expected = (false, false, "Камни с изображением рун на них, порошок из кварца");
        var actual = (spell.ComponentS, spell.ComponentV, spell.ComponentM);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParsingSchool()
    {
        Assert.Equal("ограждение", _spell.School);
    }

    [Fact]
    public void ParsingCastingTime()
    {
        Assert.Equal("1 реакция, совершаемая вами, когда вас атакуют или вы становитесь целью заклинания волшебная стрела", _spell.CastingTime);
    }

    [Fact]
    public void ParsingDistance()
    {
        Assert.Equal("На себя", _spell.Distance);
    }

    [Fact]
    public void ParsingDuration()
    {
        Assert.Equal("Мгновенная", _spell.Duration);
    }

    [Fact]
    public void ParsingClasses()
    {
        var expected = new[] { "волшебник", "колдун", "чародей" };

        Assert.Equal(expected, _spell.UnitClasses);
    }

    [Fact]
    public void ParsingSources()
    {
        var sources = new[] { "Homebrew" };

        Assert.Equal(sources, _spell.Sources);
    }

    [Fact]
    public void ParsingDescription()
    {
        var desc =
            "Когда вас атакуют или вы становитесь целью заклинания волшебная стрела [Magic missile], вы можете совершить свою реакцию, чтобы создать магический барьер, который отражает часть урона обратно к вашему нападающему. Вы уменьшаете полученный урон на 1к10 + ваш модификатор базовой характеристики, и нападающий получает урон излучением, равный количеству урона, на которое уменьшен изначальный урон.\nНа больших уровнях. Когда вы накладываете это заклинание, используя ячейку заклинания 3-го уровня или выше, уменьшение урона увеличивается на 1к10 за каждый уровень ячейки выше второго.";

        Assert.Equal(desc, _spell.Description);
    }

    [Fact]
    public async Task ParsingSpellWithoutEngTitle()
    {
        using var client = new HttpClient();
        var spell = await new SpellConstructor(client, "https://dnd.su/homebrew/spells/3590-beseda_s_prizrakom/").ConstructSpell();

        var expected = ("Беседа с призраком", string.Empty);
        var actual = (spell.Title, spell.TitleEn);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ParsingSpellWithWrongUrl()
    {
        using var client = new HttpClient();
        var spellTask = new SpellConstructor(client, "https://dnd.su/homebrew/spells/3590-someWrongUrl/").ConstructSpell();
        string actual = String.Empty;
        string expected = "Ошибка сборки заклинания: https://dnd.su/homebrew/spells/3590-someWrongUrl/";

        try
        {
            await spellTask;
        }
        catch (Exception e)
        {
            actual = e.Message;
        }

        Assert.Equal(expected, actual);
    }
}