namespace RneSniffer.Helpers
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    using System.Collections.Generic;

    public static class WebDriverHelper
    {
        public static IWebDriver CreateSession()
        {
            IWebDriver webDriver = null;
            if (SeleniumConfig.GridEnabled)
            {
                Dictionary<string, object> chromeCapability = new Dictionary<string, object>();
                chromeCapability.Add("args", new string[] { string.Format("--lang={0}", CultureInfo.CurrentCulture), "--no-sandbox" });
                DesiredCapabilities desiredCapabilities = new DesiredCapabilities("chrome", string.Empty, new OpenQA.Selenium.Platform(OpenQA.Selenium.PlatformType.Any));
                desiredCapabilities.SetCapability(ChromeOptions.Capability, chromeCapability);
                webDriver = new RemoteWebDriver(SeleniumConfig.SeleniumHubEndPoint, desiredCapabilities);
            }
            else
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument(string.Format("--lang={0}", CultureInfo.CurrentCulture));
                var chromeDriverPath = SeleniumConfig.ChromeDriverLocation;//Environment.CurrentDirectory + 
                webDriver = new ChromeDriver(chromeDriverPath, options, TimeSpan.FromSeconds(60));
            }
            return webDriver;
        }

        public static void ResizeWindow(this IWebDriver webDriver, Size? windowSize)
        {
            if (windowSize != null)
            {
                webDriver.Manage().Window.Size = windowSize.Value;
            }
            else
            {
                webDriver.Manage().Window.Maximize();
            }
        }
    }
}
