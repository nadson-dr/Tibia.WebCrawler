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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //Console.WriteLine("Capturando Mundos");
            //World.InserirWorlds();

            //List<World> lstWorlds = World.GetWorlds();

            //Console.WriteLine("Consultando Mundos");
            //Character.GetCharacters(lstWorlds);

            Console.WriteLine("Iniciando consulta de personagens");
            List<Character> lstCharacters = Character.GetCharactersFromBD();

            Character.consultarCharacter(lstCharacters);

            stopWatch.Stop();
            Console.WriteLine("Executado em: " + stopWatch.Elapsed);
            Console.ReadLine();

        }
    }
}
