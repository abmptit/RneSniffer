namespace RneSniffer.Helpers
{
    using System;
    using System.Drawing;
    using System.IO;

    public static class SeleniumConfig
    {
        public static string SeleniumHubAddress => ConfigHelper.GetStringValue("Selenium.hub.Address");

        public static string SeleniumHubPort => ConfigHelper.GetStringValue("Selenium.hub.Port");

        public static Uri SeleniumHubEndPoint => new Uri(string.Format("http://{0}:{1}/wd/hub", SeleniumConfig.SeleniumHubAddress, SeleniumConfig.SeleniumHubPort), UriKind.Absolute);

        public static bool GridEnabled => ConfigHelper.GetBoolValue("Selenium.Grid.Enabled");

        public static string LastChromeDriverVersionUrl => ConfigHelper.GetStringValue("Selenium.ChromeDriver.LastChromeDriverVersionUrl");
        public static string ChromeDriverVersion => ConfigHelper.GetStringValue("Selenium.ChromeDriver.ChromeDriverVersion");

        public static string WebDriverLocation
        {
            get
            {
                var codeLocation = System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location);
                var driverLocation = ConfigHelper.GetStringValue("Selenium.webdriver.directory");
                var chromeDriverLocation = Path.Combine(codeLocation, driverLocation);
                var directoryInfo = new DirectoryInfo(chromeDriverLocation);
                return directoryInfo.FullName;
            }
        }

        public static Size? BrowserSize => ConfigHelper.GetSizeValue("Selenium.Browser.Size");
    }
}
