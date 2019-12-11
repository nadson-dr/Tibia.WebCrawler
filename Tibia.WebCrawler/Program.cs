using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tibia.WebCrawler.Models;

namespace Tibia.WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            //World.InserirWorlds();
            //var json =JsonConvert.SerializeObject(Character.consultarCharacter());
            //Console.WriteLine(json);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //List<World> lstWorlds = World.GetWorlds();

            //foreach (World world in lstWorlds)
            //{
            //    Console.WriteLine("Consultando mundo: " + world.Name);
            //    Character.GetCharacters(world.Name);
            //}

            List<Character> lstCharacters = Character.GetCharactersFromBD();

            Character.consultarCharacter(lstCharacters);

            stopWatch.Stop();
            Console.WriteLine("Executado em: " + stopWatch.Elapsed);
            Console.ReadLine();

        }
    }
}
