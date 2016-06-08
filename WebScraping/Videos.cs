using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using RestSharp;
using Newtonsoft.Json;

namespace WebScraping
{
    class PesquisaSeries
    {
        static string vvImdbId;
        static void Main2(string[] args)
        {

            Console.WriteLine("Inicio da Captura!!");

            var baseUrl = "http://www.scnsrc.me/category/tv/page/";

            var client = new HtmlWeb();

            var paginaSeriesHome = client.Load(baseUrl + "2/");

            var titulo = paginaSeriesHome.DocumentNode.SelectSingleNode("//head/title").InnerText.Split(' ').Skip(6).First().Trim();

            var paginas = Convert.ToInt32(titulo);
            //            var paginas = 4000;

            Int64 contador = 0;

            for (int i = 1; i <= paginas; i++)
            {
                var paginaSerie = new HtmlWeb();

                var paginaSeries = paginaSerie.Load(baseUrl + i);

                var listaSerie = paginaSeries.DocumentNode.SelectNodes("//*[@id='center']/div[1]");

                Int32 la = 0;
                if (paginas == 1)
                {
                    la = 13;
                }
                else
                {
                    la = 12;
                }
                foreach (var item in listaSerie)
                {
                    for (int a = 1; a <= la; a++)
                    {
                        try
                        {
                            string tituloSerie;
                            var teste = item.SelectSingleNode("div[" + a + "]");
                            try
                            {
                                tituloSerie = teste.SelectSingleNode("div[2]/p[1]/a").InnerText;
                            }
                            catch (Exception)
                            {
                                tituloSerie = "";
                            }

                            var urlSerie = teste.SelectSingleNode("h2/a").Attributes["href"].Value;
                            string linkTorrent;
                            try
                            {
                                linkTorrent = teste.SelectSingleNode("div[2]/div/p[1]/a[1]").Attributes["href"].Value;
                            }
                            catch (Exception)
                            {
                                linkTorrent = "";
                            }
                            ++contador;

                            Console.WriteLine(contador);
                            Console.WriteLine("Url Serie      : " + urlSerie);
                            if (linkTorrent != "" && tituloSerie != "")
                            {
                                var vTitulo = tituloSerie.Split(':').First().Trim();
                                var vTemporadaEpisodio = tituloSerie.Split(':').Skip(1).First().Trim();
                                string ano;
                                string vTemporada;
                                string vEpisodio;
                                if (vTemporadaEpisodio.Contains("Season"))
                                {
                                    vTemporada = SoNumeros(vTemporadaEpisodio.Split(' ').Skip(1).First().Trim());
                                    vEpisodio = SoNumeros(vTemporadaEpisodio.Split(' ').Skip(3).First().Trim());
                                    ano = SoNumeros(tituloSerie);
                                    Console.WriteLine("Temporada      : " + vTemporada);
                                    Console.WriteLine("Episodio       : " + vEpisodio);
                                }
                                else
                                {
                                    ano = SoNumeros(tituloSerie.Split(':').Skip(1).First().Trim());
                                    vEpisodio = "0";
                                    vTemporada = "0";
                                }
                                GetImdb(vTitulo);
                                Console.WriteLine("Link Torrent   : " + linkTorrent);
                                GetTorrents(linkTorrent, vvImdbId);
                                GetLinksSeries(urlSerie, vvImdbId);

                                if (vvImdbId != null)
                                {
                                    var seriado = new SERIADO();
                                    seriado.data = ano.Trim();
                                    seriado.episodio = Convert.ToInt64(vEpisodio);
                                    seriado.serie = vvImdbId.Trim();
                                    seriado.temporada = Convert.ToInt64(vTemporada);
                                    seriado.urlserie = urlSerie.Trim();

                                    using (var conexao = new SERIEEntities())
                                    {
                                        conexao.SERIADO.Add(seriado);
                                        conexao.SaveChanges();
                                    }
                                }
                            }
                            Console.WriteLine("__________________________________________________________________________________________________________________________________________________________________________________");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine("__________________________________________________________________________________________________________________________________________________________________________________");
                        }
                    }
                }
            }
            Console.Write("Fim de Processo pressione qualquer tecla para finalizar!!");
            Console.ReadKey();
        }

        private static void GetImdb(string titulo)
        {
            var link = "http://www.omdbapi.com/?t=" + titulo + "&plot=full&r=json";
            var client = new RestClient(link);
            var request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            IMDB m = JsonConvert.DeserializeObject<IMDB>(content);

            vvImdbId = m.imdbID;

            if (m.imdbID != null)
            {
                using (var conexao = new SERIEEntities())
                {
                    conexao.IMDB.Add(m);
                    conexao.SaveChanges();
                }
            }
        }

        private static void GetTorrents(string link, string serie)
        {
            if (serie != null)
            {

                try
                {
                    var client = new HtmlWeb();
                    var doc = client.Load(link);
                    var ListaLinks = new List<LINKDOWNLOAD>();
                    foreach (HtmlNode links in doc.DocumentNode.SelectNodes("//*[@id='content']" + "//a[@href]"))
                    {
                        HtmlAttribute att = links.Attributes["href"];
                        string sub = att.Value.Substring(0, 7).ToLower().ToString();
                        if (sub == "http://")
                        {

                            var vLink = new LINKDOWNLOAD();
                            vLink.shortlink = "";
                            //                        vLink.shortlink = GetShortLink(att.Value);
                            vLink.link = att.Value;
                            vLink.serie = serie;
                            vLink.tipo = "TORRENT";

                            ListaLinks.Add(vLink);
                        }
                    }
                    Console.WriteLine("links Torrents : " + ListaLinks.Count);
                    using (var conexao = new SERIEEntities())
                    {
                        conexao.LINKDOWNLOAD.AddRange(ListaLinks);
                        conexao.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static int Somar(int mod, ref int soma)
        {
            int paginas;
            if (soma > 15)
            {
                paginas = 15;
                soma = soma - paginas;
            }
            else
            {
                paginas = mod;
                soma = soma - paginas;
            }
            return paginas;
        }

        public static string SoNumeros(string value)
        {
            return string.Join("", System.Text.RegularExpressions.Regex.Split(value, @"[^\d]"));
        }

        public static void GetLinksSeries(string link, string serie)
        {
            if (serie != null)
            {
                try
                {
                    var client = new HtmlWeb();
                    var doc = client.Load(link);

                    var ListaLinks = new List<LINKDOWNLOAD>();

                    if (doc.DocumentNode.SelectNodes("//*[@id='comment_list']" + "//a[@href]") != null)
                    {

                        foreach (HtmlNode links in doc.DocumentNode.SelectNodes("//*[@id='comment_list']" + "//a[@href]"))
                        {
                            HtmlAttribute att = links.Attributes["href"];
                            string sub = att.Value.Substring(0, 7).ToLower().ToString();
                            if (sub == "http://")
                            {
                                var vLink = new LINKDOWNLOAD();
                                vLink.shortlink = "";
                                //                        vLink.shortlink = GetShortLink(att.Value);
                                vLink.link = att.Value;
                                vLink.serie = serie;
                                vLink.tipo = "NORMAL";


                                ListaLinks.Add(vLink);
                            }
                        }
                        Console.WriteLine("links Download : " + ListaLinks.Count);
                        using (var conexao = new SERIEEntities())
                        {
                            conexao.LINKDOWNLOAD.AddRange(ListaLinks);
                            conexao.SaveChanges();
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

        }

        private static string GetShortLink(string link)
        {
            try
            {
                string preUrl = "http://ouo.io/api/Fo6n9L17?s=";
                var client = new HtmlWeb();
                var doc = client.Load(preUrl + link.Replace("http://", ""));
                return doc.DocumentNode.InnerText;

            }
            catch (Exception)
            {
                return link;
            }
        }
    }
}