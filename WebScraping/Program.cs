using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;


namespace WebScraping
{
    class PesquisaEmpresasApontador
    {
        static void Main1(string[] args)
        {
            Console.Write("Digite a busca: ");
            var empresa = Console.ReadLine();

            if (empresa.ToUpper() != "EXIT")
            {


                int contador = 0;

                var baseUrl = "http://www.apontador.com.br" + "/local/search.html?q=" + empresa + "&page=";

                var client = new HtmlWeb();

                var paginaMateriaisHome = client.Load(baseUrl);

                var paginaEmpresa = paginaMateriaisHome.DocumentNode.SelectSingleNode("/html/body/section/div[2]/p/span").InnerText;

                double x = Math.Truncate(Convert.ToDouble(paginaEmpresa) / 15) + 1;

                int mod = Convert.ToInt32(x) % 15;

                Console.WriteLine(x);

                var soma = Convert.ToInt32(paginaEmpresa.Replace(".", ""));

                for (int i = 1; i <= x; i++)
                {
                    var url = baseUrl + i;

                    var paginaMateriais = client.Load(url);

                    var materiais = paginaMateriais.DocumentNode.SelectNodes("/html/body/section/div[2]/section");

                    var ListaFarmacia = new List<Farmacia>();

                    foreach (var item in materiais)
                    {
                        int paginas = Somar(mod, ref soma);
                        for (int a = 1; a <= paginas; a++)
                        {
                            string estado;
                            string cidade;
                            string nomeCategoria;
                            string urlimagem;

                            try
                            {
                                urlimagem =
                                    item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]/h3/a").Attributes["href"].Value;
                                estado = urlimagem.Split('/').Skip(4).First().ToUpper();
                                cidade = urlimagem.Split('/').Skip(5).First().ToUpper().Replace("_", " ");
                                nomeCategoria = urlimagem.Split('/').Skip(6).First().ToUpper();
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    urlimagem =
                                        item.SelectSingleNode("/html/body/section/div[1]/section/article[" + a + "]/h2/a").Attributes["href"].Value;
                                    estado = urlimagem.Split('/').Skip(4).First().ToUpper();
                                    cidade = urlimagem.Split('/').Skip(5).First().ToUpper().Replace("_", " ");
                                    nomeCategoria = urlimagem.Split('/').Skip(6).First().ToUpper();
                                }
                                catch (Exception)
                                {
                                    urlimagem = "Não Capturado a URL";
                                    estado = "";
                                    cidade = "";
                                    nomeCategoria = "";
                                }
                            }
                            string nomeEmpresa;
                            try
                            {
                                nomeEmpresa =
                                    item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]/h3/a").InnerText;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    nomeEmpresa =
                                       item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]/h2/a").InnerText;
                                }
                                catch (Exception)
                                {
                                    nomeEmpresa = "Não Capturado o Nome";
                                }
                            }
                            string classificacao;
                            try
                            {
                                classificacao =
                                    item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]/div[1]/img").Attributes["alt"].Value;
                            }
                            catch (Exception)
                            {
                                classificacao = "0";
                            }
                            string endereco;
                            try
                            {
                                endereco =
                                item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]/p").InnerHtml;
                            }
                            catch (Exception)
                            {
                                endereco = "";
                            }
                            var cep = endereco.Split(' ').Last().ToUpper();
                            string latitude;
                            try
                            {
                                latitude = item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]").Attributes["data-place-lat"].Value;
                            }
                            catch (Exception)
                            {
                                latitude = "";
                            }
                            string longitude;
                            try
                            {
                                longitude = item.SelectSingleNode("/html/body/section/div[2]/section/article[" + a + "]").Attributes["data-place-long"].Value;
                            }
                            catch (Exception)
                            {
                                longitude = "";
                            }
                            var urlTelefone = urlimagem + "#show_phone";
                            string numeroTelefone = "";
                            string dddTelefone = "";
                            string siteEmpresa = "";
                            string horario = "";
                            if (urlimagem != "Não Capturado a URL")
                            {
                                var clientTelefone = new HtmlWeb();
                                // var urlTeste = "http://www.apontador.com.br/local/go/aparecida_de_goiania/magazines/D9425YN8/lojas_americanas_buriti_shopping.html";
                                var paginaTelefoneHome = clientTelefone.Load(urlTelefone);
                                try
                                {
                                    siteEmpresa =
                                        paginaTelefoneHome.DocumentNode.SelectSingleNode("/html/body/section/div/div[1]/div/ul/li[4]").InnerText.Replace("\n", "");
                                }
                                catch (Exception)
                                {
                                }
                                try
                                {
                                    horario =
                                        paginaTelefoneHome.DocumentNode.SelectSingleNode("/html/body/section/div/div[1]/div/ul/li[4]/span/text()").InnerText;
                                }
                                catch (Exception)
                                {
                                }
                                string numeroTelefoneDDD = "";
                                try
                                {
                                    numeroTelefoneDDD =
                                        paginaTelefoneHome.DocumentNode.SelectSingleNode(
                                            "/html/body/section/div/div[1]/div/ul/li[1]/strong").Attributes["data-content"].Value;
                                    numeroTelefone = numeroTelefoneDDD.Split(' ').Last().ToUpper().Replace("-", "");
                                    dddTelefone = numeroTelefoneDDD.Split(' ').First().ToUpper().Replace("(", "").Replace(")", "");
                                }
                                catch (Exception)
                                {
                                    numeroTelefoneDDD = "";
                                }
                            }
                            contador = ++contador;
                            var tabela = contador + " De " + paginaEmpresa + " | Nome Empresa: " + nomeEmpresa;
                            var empresaMOD = new Farmacia();
                            empresaMOD.Empresa = nomeEmpresa;
                            empresaMOD.classificacao = classificacao;
                            empresaMOD.Endereco = endereco;
                            empresaMOD.url = urlimagem;
                            empresaMOD.latitude = latitude;
                            empresaMOD.longitude = longitude;
                            empresaMOD.estado = estado;
                            empresaMOD.cidade = cidade;
                            empresaMOD.ddd = dddTelefone;
                            empresaMOD.categoria = nomeCategoria;
                            empresaMOD.cep = cep;
                            empresaMOD.telefone = numeroTelefone;
                            empresaMOD.site = siteEmpresa;
                            empresaMOD.horario = horario;
                            ListaFarmacia.Add(empresaMOD);
                            if (siteEmpresa != "")
                            {
                                Console.WriteLine(tabela);
                                Console.WriteLine("Site : " + siteEmpresa);
                                Console.WriteLine("------------------------------------------------------------------------------------------------------");
                            }
                        }
                    }
                    using (var conexao = new LEILAOEntities())
                    {
                        conexao.Farmacias.AddRange(ListaFarmacia);
                        conexao.SaveChanges();
                    }
                }
                Console.ReadKey();
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
    }
}