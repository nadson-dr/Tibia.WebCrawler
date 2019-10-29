using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Tibia.WebCrawler.Models
{
    public class Characters
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Vocation { get; set; }
        public string Level { get; set; }
        public string AchievementPoints { get; set; }
        public string World { get; set; }
        public string Residence { get; set; }
        public string House { get; set; }
        public string LastLogin { get; set; }
        public string AccountStatus { get; set; }
        public List<CharacterDeaths> characterDeaths { get; set; }

        public static Characters consultarCharacter()
        {
            Characters character = new Characters();

            List<SearchAttributes> listaAtributos = new List<SearchAttributes>();

            foreach (var info in character.GetType().GetProperties())
            {
                if (!info.PropertyType.FullName.Contains("Collections"))
                {
                    listaAtributos.Add(new SearchAttributes { text = info.Name });
                }
            }

            try
            {
                using (var driver = new ChromeDriver())
                {
                    driver.Navigate().GoToUrl("https://www.tibia.com/community/?subtopic=characters");

                    var userNameField = driver.FindElementByName("name");
                    var submitButton = driver.FindElementByXPath("//input[@name='Submit']");

                    userNameField.SendKeys("Do You Bleed");
                    submitButton.Click();

                    IList<IWebElement> tables = driver.FindElements(By.TagName("table"));

                    var table = tables.FirstOrDefault();
                    if (table != null)
                    {
                        foreach (IWebElement tr in table.FindElements(By.TagName("tr")))
                        {
                            IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

                            if (tds.Count > 0)
                            {
                                var item = listaAtributos.Where(x => x.text == tds.First().Text.Replace(":", "").Replace(" ", "")).FirstOrDefault();

                                if (item != null)
                                {
                                    var properties = character.GetType().GetProperties().Where(x => x.Name == item.text).FirstOrDefault();

                                    if (properties != null)
                                    {
                                        properties.SetValue(character, Convert.ChangeType(tds.Last().Text, Type.GetType(properties.PropertyType.FullName)), null);
                                    }
                                }
                            }
                        }
                    }

                    var result = driver.FindElementById("characters").Text;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return character;

        }

    }

    public class SearchAttributes
    {
        public string text { get; set; }

        public static List<SearchAttributes> listaAtributos()
        {
            List<SearchAttributes> listaAtributos = new List<SearchAttributes>();

            listaAtributos.Add(new SearchAttributes { text = "Name" });
            listaAtributos.Add(new SearchAttributes { text = "Sex" });
            listaAtributos.Add(new SearchAttributes { text = "Vocation" });
            listaAtributos.Add(new SearchAttributes { text = "Level" });
            listaAtributos.Add(new SearchAttributes { text = "Achievement Points" });
            listaAtributos.Add(new SearchAttributes { text = "World" });
            listaAtributos.Add(new SearchAttributes { text = "Residence" });
            listaAtributos.Add(new SearchAttributes { text = "Last Login" });
            listaAtributos.Add(new SearchAttributes { text = "Account Status" });
            listaAtributos.Add(new SearchAttributes { text = "House" });

            return listaAtributos;
        }

    }


}
