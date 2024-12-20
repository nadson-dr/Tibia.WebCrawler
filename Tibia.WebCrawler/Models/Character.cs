﻿using System;
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
    public class Character
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
        public string GuildMembership { get; set; }

        public static void consultarCharacter(List<Character> lstCharacters)
        {
            Console.WriteLine("Iniciando consulta de personagens.");
            List<SearchAttributes> listaAtributos = new List<SearchAttributes>();

            foreach (var info in new Character().GetType().GetProperties())
            {
                if (!info.PropertyType.FullName.Contains("Collections"))
                {
                    listaAtributos.Add(new SearchAttributes { text = info.Name });
                }
            }
            List<Character> lstCharacterPesquisados = new List<Character>();
            try
            {
                using (var driver = new ChromeDriver())
                {
                    
                    foreach (Character ch in lstCharacters)
                    {
                        driver.Navigate().GoToUrl("https://www.tibia.com/community/?subtopic=characters");
                        Character character = new Character();

                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                        var userNameField = wait.Until(ExpectedConditions.ElementExists(By.Name("name")));
                        var submitButton = wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@name='Submit']"))) ;

                        userNameField.SendKeys(ch.Name);
                        submitButton.Click();
                        Console.WriteLine("Consultando personagem: " + ch.Name);

                        var containerCharacters = wait.Until(ExpectedConditions.ElementExists(By.Id("characters")));
                        Console.WriteLine("Container de personagens encontrado.");

                        var table = containerCharacters.FindElements(By.TagName("table")).FirstOrDefault();

                        if (table != null && !table.Text.Contains("Could not find character"))
                        {
                            Console.WriteLine("Tabela de informações encontrada.");
                            Console.WriteLine("Capturando informações.");
                            foreach (IWebElement tr in table.FindElements(By.TagName("tr")))
                            {
                                IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

                                if (tds.Count > 0)
                                {
                                    IWebElement elemento = tds.First();

                                    var item = listaAtributos.Where(x => x.text == elemento.Text.Replace(":", "").Replace(" ", "")).FirstOrDefault();

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
                            if (!character.Name.Contains("will be deleted"))
                            {
                                lstCharacterPesquisados.Add(character);
                                Console.WriteLine("Count char: " + lstCharacterPesquisados.Count());
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro:" + ex.Message);
                
            }
            finally
            {
                using (var crudDao = new crudDAO())
                {
                    DataTable dt = new DataTable();
                    dt = crudDao.CreateDataTable(lstCharacterPesquisados);


                }

                foreach (Character chPesquisado in lstCharacterPesquisados)
                {
                    Console.WriteLine("Inserindo/Alterando personagens: " + chPesquisado.Name);
                    InserirAlterarCharacter(chPesquisado);
                }
            }
        }
        public static void InserirAlterarCharacter(Character character)
        {
            try
            {
                Dictionary<string, object> dicParametros = new Dictionary<string, object>();

                #region " Query + Parâmetros " 

                string query = @"if exists (select * from Characters where Name = @Name)
	                                    UPDATE [dbo].[Characters]
	                                       SET [Name] = @Name
		                                      ,[Sex] = @Sex
		                                      ,[Vocation] = @Vocation
		                                      ,[CharacterLevel] = @CharacterLevel
		                                      ,[AchievementPoints] = @AchievementPoints
		                                      ,[World] = @World
		                                      ,[Residence] = @Residence
		                                      ,[House] = @House
		                                      ,[LastLogin] = @LastLogin
		                                      ,[AccountStatus] = @AccountStatus
		                                      ,[GuildMembership] = @GuildMembership
	                                     WHERE [Name] = @Name
                                    else 
	                                    INSERT INTO [dbo].[Characters]
				                                    ([Name] ,[Sex] ,[Vocation]
				                                    ,[CharacterLevel] ,[AchievementPoints] ,[World]
				                                    ,[Residence] ,[House] ,[LastLogin]
				                                    ,[AccountStatus] ,[GuildMembership])
			                                    VALUES
				                                    (@Name ,@Sex ,@Vocation
				                                    ,@CharacterLevel ,@AchievementPoints ,@World
				                                    ,@Residence ,@House ,@LastLogin
				                                    ,@AccountStatus ,@GuildMembership)";

                dicParametros.Add("@Name", character.Name);
                dicParametros.Add("@Sex", character.Sex);
                dicParametros.Add("@Vocation", character.Vocation);
                dicParametros.Add("@CharacterLevel", character.Level);
                dicParametros.Add("@AchievementPoints", character.AchievementPoints);
                dicParametros.Add("@World", character.World);
                dicParametros.Add("@Residence", character.Residence);
                dicParametros.Add("@House", character.House);
                dicParametros.Add("@LastLogin", (string.IsNullOrEmpty(character.LastLogin) ? DBNull.Value : (object)Convert.ToDateTime(character.LastLogin.Replace("CET", ""))));
                dicParametros.Add("@AccountStatus", character.AccountStatus);
                dicParametros.Add("@GuildMembership", character.GuildMembership);

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
        public static void InserirAlterarCharacterWorld(Character character)
        {
            try
            {
                Dictionary<string, object> dicParametros = new Dictionary<string, object>();

                #region " Query + Parâmetros " 

                string query = @"if exists (select * from Characters where Name = @Name)
	                                    UPDATE [dbo].[Characters]
	                                       SET [Name] = @Name
		                                      ,[Vocation] = @Vocation
		                                      ,[CharacterLevel] = @CharacterLevel
                                              ,[World] = @World
	                                     WHERE [Name] = @Name
                                    else 
	                                    INSERT INTO [dbo].[Characters]
				                                    ([Name], [Vocation]
				                                    ,[CharacterLevel], [World])
			                                    VALUES
				                                    (@Name, @Vocation
				                                    ,@CharacterLevel, @World)";

                dicParametros.Add("@Name", character.Name);
                dicParametros.Add("@Vocation", character.Vocation);
                dicParametros.Add("@CharacterLevel", character.Level);
                dicParametros.Add("@World", character.World);

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
        public static void GetCharacters(List<World> lstWorlds)
        {
            List<Character> lstCharacters = new List<Character>();

            Console.WriteLine("Iniciando consulta de personagens por mundo.");

            try
            {
                using (var driver = new ChromeDriver())
                {
                    foreach (World world in lstWorlds)
                    {
                        driver.Navigate().GoToUrl("https://www.tibia.com/community/?subtopic=worlds&world=" + world.Name);
                        Console.WriteLine("Consultando mundo: " + world.Name);
                        
                        var container = driver.FindElement(By.ClassName("Table2"));
                        Console.WriteLine("Container Capturado.");

                        var tables = container.FindElements(By.TagName("table"));
                        Console.WriteLine("Tabelas Capturadas.");

                        var trs = tables[0].FindElements(By.TagName("tr"));
                        Console.WriteLine("Rows Capturadas.");

                        for (int i = 1; i < trs.Count; i++)
                        {

                            Character character = new Character();
                            var tds = trs[i].FindElements(By.TagName("td"));

                            character.Name = tds[0].Text;
                            character.Level = tds[1].Text;
                            character.Vocation = tds[2].Text;
                            character.World = world.Name;
                            Console.WriteLine("Personagem Adicionado: " + character.Name);

                            lstCharacters.Add(character);
                        }
                    }
                }

                foreach (var character in lstCharacters)
                {
                    Console.WriteLine("Insedrindo Personagem: " + character.Name + " do " + character.World);
                    InserirAlterarCharacterWorld(character);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                Console.ReadLine();
                throw;
            }
        }
        public static List<Character> GetCharactersFromBD()
        {
            List<Character> lstCharacters = new List<Character>();

            try
            {
                DataSet _ds = new DataSet();

                #region " Query + Parâmetros " 

                string query = @"SELECT [Id]
                              ,[Name]
                              ,[Sex]
                              ,[Vocation]
                              ,[CharacterLevel] as [Level]
                              ,[AchievementPoints]
                              ,[World]
                              ,[Residence]
                              ,[House]
                              ,[LastLogin]
                              ,[AccountStatus]
                              ,[GuildMembership]
                          FROM [bd_TibiaRolePlay].[dbo].[Characters]
                          where sex is null";

                #endregion

                using (crudDAO objDAO = new crudDAO())
                {
                    _ds = objDAO.listarQuery(query);
                }

                foreach (DataRow _dr in _ds.Tables[0].Rows)
                {
                    Character character = (Character)Utils.Utils.preencherObjeto(typeof(Character), _dr);
                    lstCharacters.Add(character);
                }

                return lstCharacters;
            }
            catch(Exception ex)
            {
                return lstCharacters;
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
                listaAtributos.Add(new SearchAttributes { text = "Guild Membership" });


                return listaAtributos;
            }

        }

    }
}
