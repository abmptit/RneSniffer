using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using RneSniffer.Helpers;
using RneSniffer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RneSniffer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WebDriverHelper.EnsureChromeDriverAsync();

            var annee = (args != null && args.Count() > 0) ? args[0] : "2020";
            RecupererSociete(annee);
        }

        //static async Task Main(string[] args)
        //{
        //    MergeFichiersAnnee("2018");
        //}

        //private static void RecupererSocieteDepuisJort()
        //{
        //    List<EntrepriseModel> entrepriseModels = new List<EntrepriseModel>();
        //    string rootUrlRNE = "https://www.registre-entreprises.tn/search/";
        //    string urlJort = $"{rootUrlRNE}SearchBorne.do?action=searchBorne";
        //    string urlRNENext = $"{rootUrlRNE}RCCSearch.do?action=search&id=n";
        //    string urlRNEPrevious = $"{rootUrlRNE}RCCSearch.do?action=search&id=p";
        //    string urlDetailsResult = $"{rootUrlRNE}SearchBorne.do?action=searchDetails&borneIndex=";
        //    string urlBackToList = $"{rootUrlRNE}SearchBorne.do?action=searchResult";

        //    using (var webDriver = WebDriverHelper.CreateSession())
        //    {
        //        webDriver.ResizeWindow(SeleniumConfig.BrowserSize);
        //        webDriver.Navigate().GoToUrl(urlJort);
        //        Thread.Sleep(500);
        //        var selectTypePublication = webDriver.FindElement(By.Name("searchObject.typePublication"));
        //        selectTypePublication.Click();
        //        var optionCreation = selectTypePublication.FindElement(By.XPath("option[@value='Creation']"));
        //        optionCreation.Click();
        //        var anneeFrom = webDriver.FindElement(By.Name("searchObject.dateFrom"));
        //        anneeFrom.SendKeys("01/01/2020");
        //        var anneeTo = webDriver.FindElement(By.Name("searchObject.dateTo"));
        //        anneeTo.SendKeys("31/12/2020");
        //        var searchButton = webDriver.FindElement(By.ClassName("search-jort-btn"));
        //        WebDriverWait wait = new WebDriverWait(webDriver, new TimeSpan(5000));
        //        wait.Until(ExpectedConditions.ElementToBeClickable(searchButton));
        //        searchButton.Click();

        //        bool isElementDisplayed = webDriver.FindElement(By.ClassName("search-result-jort")).Displayed;

        //        int indexFile = 0;

        //        while (webDriver.FindElement(By.ClassName("search-result-jort")).Displayed)
        //        {
        //            var lignes = webDriver.FindElements(By.XPath("//table[@class='search-result-jort']/tbody/tr"));

        //            for (int i = 0; i < lignes.Count(); i++)
        //            {
        //                var entreprise = new EntrepriseModel();
        //                webDriver.Navigate().GoToUrl(urlDetailsResult + $"{i}");

        //                if (webDriver.FindElement(By.ClassName("search-result-jort")).Displayed)
        //                {
        //                    entreprise.IdentifiantUnique = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][2]/div[2]/span")).Text;
        //                    entreprise.PrenomResponsable = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][3]/div[1]/span[2]")).Text;
        //                    entreprise.PrenomResponsableArabe = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][3]/div[2]/span[3]")).Text;
        //                    entreprise.NomResponsable = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][4]/div[1]/span[2]")).Text;
        //                    entreprise.NomResponsableArabe = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][4]/div[2]/span[3]")).Text;
        //                    entreprise.NomCommercial = webDriver.FindElement(By.XPath("")).Text;
        //                    entreprise.NomCommercialArabe = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][5]/div[2]/span[3]")).Text;
        //                    entreprise.NomCommercial = webDriver.FindElement(By.XPath("")).Text;
        //                    entreprise.NomCommercialArabe = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][5]/div[2]/span[3]")).Text;
        //                    entreprise.NomCommercial = webDriver.FindElement(By.XPath("")).Text;
        //                    entreprise.NomCommercialArabe = webDriver.FindElement(By.XPath("//div[@class='infojort-results-items'][5]/div[2]/span[3]")).Text;
        //                }

        //                entrepriseModels.Add(entreprise);
        //                webDriver.Navigate().GoToUrl(urlBackToList);

        //                if (entrepriseModels.Count() >= 100)
        //                {
        //                    var json = JsonConvert.SerializeObject(entrepriseModels);
        //                    File.WriteAllText($"export_{indexFile}.json", json, Encoding.UTF8);
        //                    entrepriseModels.Clear();
        //                    indexFile++;
        //                }
        //            }
        //        }
        //    }
        //}
        private static void MergeFichiersAnnee(string annee)
        {
            List<EntrepriseModel> entreprises = new List<EntrepriseModel>();
            var files = Directory.GetFiles($"backup\\fichiersBrut\\{annee}", $"*_{annee}_*.json");
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var entreprisesFiles = JsonConvert.DeserializeObject<List<EntrepriseModel>>(content);
                foreach (var entrepriseFile in entreprisesFiles)
                {
                    if (entreprises.Any(e => e.IdentifiantUnique == entrepriseFile.IdentifiantUnique))
                    {
                        // doublons
                    }
                    else
                    {
                        entreprises.Add(entrepriseFile);
                    }
                }
            }

            var mergedContent = JsonConvert.SerializeObject(entreprises);
            File.WriteAllText($"backup\\rne\\{annee}\\EntreprisesRNE_{annee}.json", mergedContent);            
        }

        private static void RecupererSociete(string annee)
        {
            List<EntrepriseModel> entrepriseModels = new List<EntrepriseModel>();
            string rootUrlRNE = "https://www.registre-entreprises.tn/search/";
            string urlRNE = $"{rootUrlRNE}RCCSearch.do?action=getPage&rg_type=PM&search_mode=NORMAL";
            string urlRNENext = $"{rootUrlRNE}RCCSearch.do?action=search&id=n";
            string urlRNEPrevious = $"{rootUrlRNE}RCCSearch.do?action=search&id=p";
            string urlDetailsResult = $"{rootUrlRNE}RCCSearch.do?action=chooseDocuments&numRegistreIndex=";
            string urlBackToList = $"{rootUrlRNE}RCCSearch.do?action=backToList";

            using (var webDriver = WebDriverHelper.CreateSession())
            {
                webDriver.ResizeWindow(SeleniumConfig.BrowserSize);
                webDriver.Navigate().GoToUrl(urlRNE);
                Thread.Sleep(500);
                var selectAnneeInsertion = webDriver.FindElement(By.Name("searchRegistrePmRcc.registrePm.demande.anneeInsertion"));
                selectAnneeInsertion.Click();
                var option2020 = selectAnneeInsertion.FindElement(By.XPath($"option[@value='{annee}']"));
                option2020.Click();
                var searchButton = webDriver.FindElement(By.XPath("//div[text() = 'Rechercher']"));
                searchButton.Click();

                bool isElementDisplayed = webDriver.FindElement(By.ClassName("search-result-jort")).Displayed;

                int indexFile = 0;

                while (webDriver.FindElement(By.ClassName("search-result-jort")).Displayed)
                {
                    var lignes = webDriver.FindElements(By.XPath("//table[@class='search-result-jort']/tbody/tr"));

                    for (int i = 0; i < lignes.Count(); i++)
                    {
                        
                        webDriver.Navigate().GoToUrl(urlDetailsResult + $"{i}");

                        try
                        {
                            var entreprise = new EntrepriseModel();

                            if (webDriver.FindElement(By.ClassName("search-result-jort")).Displayed)
                            {
                                entreprise.IdentifiantUnique = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[1]/td[2]")).Text;
                                entreprise.FormeJuridique = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[2]/td[2]")).Text;
                                entreprise.FormeJuridiqueArabe = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[2]/td[3]")).Text;
                                entreprise.EtatRegistre = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[3]/td[2]")).Text;
                                entreprise.EtatRegistreArabe = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[3]/td[3]")).Text;
                                entreprise.SituationFiscale = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[4]/td[2]")).Text;
                                entreprise.SituationFiscaleArabe = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[4]/td[3]")).Text;
                                entreprise.DenominationSociale = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[5]/td[2]")).Text;
                                entreprise.DenominationSocialeArabe = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[5]/td[3]")).Text;
                                entreprise.NomCommercial = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[6]/td[2]")).Text;
                                entreprise.NomCommercialArabe = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[6]/td[3]")).Text; ;
                                entreprise.AdresseSiege = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[7]/td[2]")).Text;
                                entreprise.AdresseSiegeArabe = webDriver.FindElement(By.XPath("//table[contains(@class,'search-result-jort')]/tbody/tr[7]/td[3]")).Text;
                            }

                            entrepriseModels.Add(entreprise);
                        }
                        catch(Exception ex)
                        {

                        }

                        webDriver.Navigate().GoToUrl(urlBackToList);
                    }

                    if (entrepriseModels.Count() >= 1000)
                    {
                        var json = JsonConvert.SerializeObject(entrepriseModels);
                        File.WriteAllText($"export_{annee}_{indexFile}.json", json, Encoding.UTF8);
                        entrepriseModels.Clear();
                        indexFile++;
                    }

                    webDriver.Navigate().GoToUrl(urlRNENext);
                }
            }
        }
    }
}
