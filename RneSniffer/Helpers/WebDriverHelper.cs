namespace RneSniffer.Helpers
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.IO.Compression;
    using NLog;

    public static class WebDriverHelper
    {

        private static readonly Logger _log_ = LogManager.GetCurrentClassLogger();

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
                var chromeDriverPath = SeleniumConfig.WebDriverLocation;//Environment.CurrentDirectory + 
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

        public static async Task EnsureChromeDriverAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string chromeVersion = string.Empty;
                if (string.IsNullOrEmpty(SeleniumConfig.ChromeDriverVersion))
                {
                    var response = await client.GetAsync(SeleniumConfig.LastChromeDriverVersionUrl);
                    chromeVersion = await response.Content.ReadAsStringAsync();
                    _log_.Info($"The last chrome driver: {SeleniumConfig.LastChromeDriverVersionUrl} {chromeVersion}");
                }
                else
                {
                    chromeVersion = SeleniumConfig.ChromeDriverVersion;
                    _log_.Info($"The chrome driver: {chromeVersion}");
                }

                var path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
                var zipFile = $"chromedriver_{chromeVersion}_win32.zip";

                _log_.Info($"check if {Path.Combine(path, zipFile)} exist");
                _log_.Info($"check if {Path.Combine(path, SeleniumConfig.WebDriverLocation, "chromedriver.exe")} exist");
                if (
                    !File.Exists(Path.Combine(path, zipFile)) ||
                    !File.Exists(Path.Combine(path, SeleniumConfig.WebDriverLocation, "chromedriver.exe"))
                    )
                {
                    using (var cli = new WebClient())
                    {
                        var zipUrl = $"https://chromedriver.storage.googleapis.com/{chromeVersion}/chromedriver_win32.zip";
                        _log_.Info($"Download driver zip file from {zipUrl}");
                        var bytes = await Task.Run(() => cli.DownloadData(zipUrl));

                        using (var stream = File.Create(Path.Combine(path, zipFile)))
                        {
                            await stream.WriteAsync(bytes, 0, bytes.Length);
                        }
                    }

                    FileInfo zipFileInfo = new FileInfo(Path.Combine(path, zipFile));

                    _log_.Info($"Unzipping into {zipFileInfo.FullName}");

                    ZipFile.ExtractToDirectory(zipFileInfo.FullName, SeleniumConfig.WebDriverLocation, System.Text.Encoding.ASCII, true);
                }
                else
                {
                    _log_.Info($"{Path.Combine(path, SeleniumConfig.WebDriverLocation, "chromedriver.exe")} already exist");
                }
            }
        }

    }
}
