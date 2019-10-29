using Newtonsoft.Json;
using System;
using Tibia.WebCrawler.Models;

namespace Tibia.WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var json =JsonConvert.SerializeObject(Characters.consultarCharacter());
            Console.WriteLine(json);
        }
    }
}
