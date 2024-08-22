using OpenQA.Selenium;

namespace DnDSu;

internal class SeleniumSpellLinksParser : IDisposable
{
    private readonly IWebDriver _driver = WebDriversFabric.GetDriver();

    public async Task<IEnumerable<string>> Parse(string url)
    {
        _driver.Navigate().GoToUrl(url);

        await LoadAllPageElements();

        return GetLinks();
    }

    private IEnumerable<string> GetLinks()
        => _driver.FindElements(By.XPath("""//a[@class="cards_list__item-wrapper"]""")).Select(x => x.GetAttribute("href"));

    private async Task LoadAllPageElements()
    {
        var scrollDelay = 50;
        var footer = _driver.FindElement(By.TagName("footer"));

        for (int i = 0; i < 10; i++)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", footer);
            await Task.Delay(scrollDelay);
        }
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}