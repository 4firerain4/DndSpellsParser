using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace DnDSu;

internal static class WebDriversFabric
{
    public static IWebDriver GetDriver()
    {
        var browser = GetBrowser();
        if (browser == "Chrome")
            return GetChromeDriver();
        if (browser == "Firefox")
            return GetFirefoxDriver();
        if (browser == "Edge")
            return GetEdgeDriver();
        
        throw new Exception("Browser not detected");
    }

    private static string GetBrowser()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (Directory.Exists(@"C:\Program Files\Google\Chrome"))
                return "Chrome";
            if (Directory.Exists(@"C:\Program Files\Mozilla Firefox"))
                return "Firefox";
            return "Edge";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (File.Exists("/usr/bin/chromium"))
                return "Chrome";
            return "Firefox";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (Directory.Exists("/Applications/Google Chrome.app"))
                return "Chrome";
            if (Directory.Exists("/Applications/Firefox.app"))
                return "Firefox";
        }

        return "undefined";
    }

    private static IWebDriver GetEdgeDriver()
    {
        var service = EdgeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        service.EnableVerboseLogging = false;
        service.SuppressInitialDiagnosticInformation = true;

        var options = new EdgeOptions();
        options.AddArgument("headless");
        options.AddArgument("window-size=1200x600");
        options.AddArgument("--disable-logging");
        options.AddArgument("--log-level=3");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--mute-audio");

        return new EdgeDriver(service, options);
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
        options.AddArgument("--no-sandbox");

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