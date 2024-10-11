using OpenQA.Selenium;

namespace DnDSu;

internal class SeleniumSpellLinksParser : IDisposable
{
    private readonly IWebDriver _driver = WebDriversFabric.GetDriver();

    public async Task<string[]> GetLinksFromUrl(string url)
    {
        await _driver.Navigate().GoToUrlAsync(url);
        await WaitToLoad();
        _driver.Manage().Cookies.AddCookie(new Cookie("spells_listLength", "10000"));
        await _driver.Navigate().RefreshAsync();
        await WaitToLoad();

        return GetLinks().ToArray();
    }

    private IEnumerable<string> GetLinks()
        => _driver.FindElements(By.XPath("""//a[@class="cards_list__item-wrapper"]""")).Select(x => x.GetAttribute("href"));

    private async Task WaitToLoad() // Может быть вечное состояние загрузки. Ждём просто появления блока.
    {
        while (true)
        {
            try
            {
                _driver.FindElement(By.ClassName("cards_list__item-name"));
                break;
            }
            catch (NoSuchElementException)
            {
                await Task.Delay(500);
            }
        }
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}