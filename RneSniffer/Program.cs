using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using RneSniffer.Helpers;
using RneSniffer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RneSniffer
{
    class Program
    {
        static void Main(string[] args)
        {
            RecupererSociete();
        }

        private static void RecupererSociete()
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
                var option2020 = selectAnneeInsertion.FindElement(By.XPath("option[@value='2020']"));
                option2020.Click();
                var searchButton = webDriver.FindElement(By.XPath("//div[text() = 'Rechercher']"));
                searchButton.Click();

                bool isElementDisplayed = webDriver.FindElement(By.ClassName("search-result-jort")).Displayed;


                while (webDriver.FindElement(By.ClassName("search-result-jort")).Displayed)
                {
                    var lignes = webDriver.FindElements(By.XPath("//table[@class='search-result-jort']/tbody/tr"));

                    for (int i = 0; i < 10; i++)
                    {
                        var ligne = lignes[i];
                        var colonnes = ligne.FindElements(By.TagName("td"));
                        var entreprise = new EntrepriseModel()
                        {
                            DenominationSociale = colonnes[0]?.Text,
                            NomCommercial = colonnes[1]?.Text,
                            NomCommercialArabe = colonnes[2]?.Text,
                            DenominationSocialeArabe = colonnes[3]?.Text,
                        };
                        webDriver.Navigate().GoToUrl(urlDetailsResult+$"{i}");

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
                        webDriver.Navigate().GoToUrl(urlBackToList);
                    }

                    webDriver.Navigate().GoToUrl(urlRNENext);
                }
            }

        }
    }
}
