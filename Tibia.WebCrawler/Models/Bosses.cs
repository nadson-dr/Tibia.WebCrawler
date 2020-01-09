using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Tibia.WebCrawler.Utils;

namespace Tibia.WebCrawler.Models
{
    public class Bosses
    {
        #region Atributos

        public string name { get; set; }

        #endregion

        #region Metodos

        public static void ConsultarBosses()
        {
            Console.WriteLine("Iniciando consulta de bosses.");
            List<Bosses> lstBosses = new List<Bosses>();

            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://www.tibiawiki.com.br/wiki/Bosses");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                var containerBosses = wait.Until(ExpectedConditions.ElementExists(By.Id("tabelaDPL")));

                if (containerBosses != null)
                {
                    foreach (IWebElement tr in containerBosses.FindElements(By.TagName("tr")))
                    {
                        Bosses boss = new Bosses();

                        IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

                        if (tds.Count > 0)
                        {
                            IWebElement elemento = tds.First();

                            boss.name = elemento.Text;
                            InserirAlterarBoss(boss);
                        }
                    }
                }

            }
        }

        public static void InserirAlterarBoss(Bosses boss)
        {
            try
            {
                Dictionary<string, object> dicParametros = new Dictionary<string, object>();

                #region " Query + Parâmetros " 

                string query = @"if exists (select * from tb_bosses where name = @Name)
	                                    UPDATE [dbo].[tb_bosses]
	                                       SET [name] = @Name
	                                     WHERE [name] = @Name
                                    else 
	                                    INSERT INTO [dbo].[tb_bosses]
				                                    ([Name])
			                                    VALUES
				                                    (@Name)";

                dicParametros.Add("@Name", boss.name);
             
                #endregion

                using (crudDAO objDAO = new crudDAO())
                {
                    objDAO.inserirAlterarExcluir(query, dicParametros);
                    dicParametros.Clear();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion
    }
}
