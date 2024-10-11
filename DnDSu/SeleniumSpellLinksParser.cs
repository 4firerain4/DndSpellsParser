using OpenQA.Selenium;

namespace DnDSu;

internal class SeleniumSpellLinksParser : IDisposable
{
    private readonly IWebDriver _driver = WebDriversFabric.GetDriver();

    public async Task<string[]> GetLinksFromUrl(string url)
    {
        await _driver.Navigate().GoToUrlAsync(url);
        _driver.Manage().Cookies.AddCookie(new Cookie("spells_listLength", "10000"));
        _driver.Navigate().Refresh();

        return GetLinks().ToArray();
    }

    private IEnumerable<string> GetLinks()
        => _driver.FindElements(By.XPath("""//a[@class="cards_list__item-wrapper"]""")).Select(x => x.GetAttribute("href"));

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}