using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WebScraping
{
    class UniverseSeries
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio da Captura!!");

            var baseUrl = "http://universeseries.com.br/page/";

            var client = new HtmlWeb();

            for (int i = 1; i < 300; i++)
            {

                var paginaUniverse = client.Load(baseUrl + i);

                var vseries = paginaUniverse.DocumentNode.SelectSingleNode("/html/body/div[1]/div[4]/div[1]/div[1]");

                var teste = from el in ParseLinks(vseries.InnerHtml)
                            where el.Contains("download")
                            where !el.Contains("#")
                            select el;

                var listaLink = new List<string>(teste);

                foreach (var item in listaLink)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine("total de links : " + listaLink.Count);
                Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            }
            Console.ReadKey();

        }

        private static List<string> ParseLinks(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");

            return nodes == null ? new List<string>() : nodes.ToList().ConvertAll(
                   r => r.Attributes.ToList().ConvertAll(
                   i => i.Value)).SelectMany(j => j).ToList();



        }

    }
}
