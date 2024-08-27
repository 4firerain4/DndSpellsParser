using Xunit;

namespace DnDSu.UnitTests;

public class SeleniumLinksParserTests
{
    [Fact]
    public async Task ParsingHandbookLinksCount()
    {
        using var handbook = new SeleniumSpellLinksParser();
        var linksCount = (await handbook.Parse("https://dnd.su/spells/")).Count();
        
        Assert.InRange(linksCount, 524, 1000); // Не менее 524 официальных заклинания (может ещё добавятся со временем)
    }
    
    [Fact]
    public async Task ParsingHomebrewLinksCount()
    {
        using var handbook = new SeleniumSpellLinksParser();
        var linksCount = (await handbook.Parse("https://dnd.su/homebrew/spells/")).Count();
        
        Assert.InRange(linksCount, 1716, 2000);
    }       
    
}