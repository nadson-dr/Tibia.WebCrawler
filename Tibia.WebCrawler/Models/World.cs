using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Tibia.WebCrawler.Utils;

namespace Tibia.WebCrawler.Models
{
    public class World
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string PvpType { get; set; }


        public static void InserirWorlds()
        {
            try
            {
                List<World> worlds = new List<World>();
                using (var driver = new ChromeDriver())
                {
                    driver.Navigate().GoToUrl("https://www.tibia.com/community/?subtopic=worlds");

                    var container = driver.FindElement(By.Id("worlds"));

                    var tables = container.FindElements(By.ClassName("TableContent"));

                    var trs = tables[2].FindElements(By.TagName("tr"));

                    for (int i = 1; i < trs.Count; i++)
                    {
                        World world = new World();
                        var tds = trs[i].FindElements(By.TagName("td"));

                        world.Name = tds[0].Text;
                        world.Location = tds[2].Text;
                        world.PvpType = tds[3].Text;

                        worlds.Add(world);
                    }
                }

                foreach (var world in worlds)
                {
                    InserirAlterarWorld(world);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void InserirAlterarWorld(World world)
        {
            try
            {
                Dictionary<string, object> dicParametros = new Dictionary<string, object>();

                #region " Query + Parâmetros " 

                string query = @"if exists (select * from Worlds where Name = @Name)
	                                    UPDATE [dbo].[Worlds]
	                                       SET [Name] = @Name
		                                      ,[Location] = @Location
		                                      ,[PvpType] = @PvpType
	                                     WHERE [Name] = @Name
                                    else 
	                                    INSERT INTO [dbo].[Worlds]
				                                    ([Name] ,[Location] ,[PvpType])
			                                   VALUES
				                                    (@Name ,@Location ,@PvpType)";

                dicParametros.Add("@Name", world.Name);
                dicParametros.Add("@Location", world.Location);
                dicParametros.Add("@PvpType", world.PvpType);

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

        public static List<World> GetWorlds()
        {
            try
            {
                List<World> lstWorlds = new List<World>();

                DataSet _ds = new DataSet();
    
                #region " Query + Parâmetros " 

                string query = @"SELECT [Name]
                                      ,[Location]
                                      ,[PvpType]
                                  FROM [bd_TibiaRolePlay].[dbo].[Worlds]";

                #endregion

                using (crudDAO objDAO = new crudDAO())
                {
                    _ds = objDAO.listarQuery(query);
                }

                foreach (DataRow _dr in _ds.Tables[0].Rows)
                {
                    World world = (World)Utils.Utils.preencherObjeto(typeof(World), _dr);
                    lstWorlds.Add(world);
                }

                return lstWorlds;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
