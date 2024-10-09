using OpenQA.Selenium;

namespace DnDSu;

internal class SeleniumSpellLinksParser : IDisposable
{
    private readonly IWebDriver _driver = WebDriversFabric.GetDriver();

    public async Task<string[]> GetLinksFromUrl(string url)
    {
        await _driver.Navigate().GoToUrlAsync(url);

        await LoadAllPageElements();

        return GetLinks().ToArray();
    }

    private IEnumerable<string> GetLinks()
        => _driver.FindElements(By.XPath("""//a[@class="cards_list__item-wrapper"]""")).Select(x => x.GetAttribute("href"));

    private async Task LoadAllPageElements()
    {
        const int scrollDelay = 100;
        const int scrollCount = 15;
        
        var footer = _driver.FindElement(By.TagName("footer"));

        for (int i = 0; i < scrollCount; i++) // Загрузка заклинаний при прокрутке
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