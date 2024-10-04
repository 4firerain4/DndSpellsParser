using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace DnDSu;

internal static class WebDriversFabric
{
    public static IWebDriver GetDriver() //TODO: поиск подходящего драйвера
    {
        try
        {
            return GetFirefoxDriver();
        }
        catch
        {
            return GetChromeDriver();
        }
    }

    private static IWebDriver GetChromeDriver()
    {
        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        service.EnableVerboseLogging = false;
        service.SuppressInitialDiagnosticInformation = true;

        var options = new ChromeOptions();
        options.AddArgument("headless");
        options.AddArgument("window-size=1200x600");
        options.AddArgument("--disable-logging");
        options.AddArgument("--log-level=3");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--mute-audio");

        return new ChromeDriver(service, options);
    }

    private static IWebDriver GetFirefoxDriver()
    {
        var mozService = FirefoxDriverService.CreateDefaultService();
        mozService.HideCommandPromptWindow = true;
        mozService.SuppressInitialDiagnosticInformation = true;
        mozService.LogLevel = FirefoxDriverLogLevel.Fatal;

        Console.SetError(TextWriter.Null);

        var mozOptions = new FirefoxOptions();
        mozOptions.AddArgument("--headless");
        mozOptions.AddArgument("--safe-mode");

        mozOptions.LogLevel = FirefoxDriverLogLevel.Default;
        mozOptions.SetLoggingPreference(LogType.Browser, LogLevel.Off);
        mozOptions.SetLoggingPreference(LogType.Client, LogLevel.Off);
        mozOptions.SetLoggingPreference(LogType.Driver, LogLevel.Off);
        mozOptions.SetLoggingPreference(LogType.Server, LogLevel.Off);
        mozOptions.SetLoggingPreference(LogType.Performance, LogLevel.Off);
        mozOptions.SetLoggingPreference(LogType.Profiler, LogLevel.Off);

        return new FirefoxDriver(mozService, mozOptions);
    }
}