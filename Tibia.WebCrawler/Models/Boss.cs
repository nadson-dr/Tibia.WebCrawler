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
    public class Boss
    {
        #region Atributos

        public int id { get; set; }
        public string name { get; set; }
        public DateTime kill_date { get; set; }
        public string world { get; set; }
        public int days_after_last { get; set; }

        #endregion

        #region Metodos

        public static void consultarBoss()
        {
            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://www.tibiabosses.com/login/");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                var userNameField = wait.Until(ExpectedConditions.ElementExists(By.Id("wpum_loginformuser_login")));
                var passwordField = wait.Until(ExpectedConditions.ElementExists(By.Id("wpum_loginformuser_pass")));
                var submitButton = wait.Until(ExpectedConditions.ElementExists(By.Id("wpum_loginformwp-submit")));


                userNameField.SendKeys("Joked");
                passwordField.SendKeys("panico23");
                submitButton.Click();

                List<World> worlds = World.GetWorlds();
                List<Boss> bosses = new List<Boss>();
                foreach (var world in worlds)
                {
                    driver.Navigate().GoToUrl("https://www.tibiabosses.com/" + world.Name + "/history/");

                    var divBosses = driver.FindElements(By.ClassName("execphpwidget"));

                    var divBossesText = divBosses[1];

                    var x = divBossesText.Text.Trim();

                    List<string> listString = x.Split("\r\n").ToList();

                    listString.RemoveRange((listString.Count() - 5), 5);
                    var nameBoss = "";
                    for (int i = 0; i < listString.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            nameBoss = listString[i];
                        }
                        else
                        {
                            List<string> listDatas = listString[i].Split("|").ToList();
                            listDatas = listDatas.Where(y => y != "" && y.Contains("-") && y.Trim() != "0000-00-00").ToList();

                            foreach (var data in listDatas)
                            {
                                bosses.Add(new Boss
                                {
                                    name = nameBoss,
                                    kill_date = Convert.ToDateTime(data),
                                    world = world.Name
                                }); ;
                            }
                        }
                    }
                }
                InserirAlterarBossKill(bosses);
            }

        }


        public static void InserirAlterarBossKill(List<Boss> bosses)
        {
            try
            {
                using (crudDAO objDAO = new crudDAO())
                {
                    DataTable dados = objDAO.CreateDataTable(bosses);

                    DataSet dsDados = new DataSet();
                    dsDados.Tables.Add(dados);

                    objDAO.inserirBulkCopy(dsDados, "tb_kill_bosses");
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
