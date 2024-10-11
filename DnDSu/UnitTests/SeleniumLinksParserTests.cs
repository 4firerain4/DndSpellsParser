using Xunit;

namespace DnDSu.UnitTests;

public class SeleniumLinksParserTests
{
    [Fact]
    public async Task ParsingHandbookLinksCount()
    {
        using var parser = new SeleniumSpellLinksParser();
        var linksCount = (await parser.GetLinksFromUrl("https://dnd.su/spells/")).Length;
        
        Assert.InRange(linksCount, 524, 1000); // Не менее 524 официальных заклинания (может ещё добавятся со временем)
    }
    
    [Fact]
    public async Task ParsingHomebrewLinksCount()
    {
        using var parser = new SeleniumSpellLinksParser();
        var linksCount = (await parser.GetLinksFromUrl("https://dnd.su/homebrew/spells/")).Length;
        
        Assert.InRange(linksCount, 1716, 2000);
    }       
    
}