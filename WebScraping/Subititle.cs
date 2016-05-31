using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Xml;


namespace WebScraping
{

    public class Subititle
    {
        public static string aToken;

        public static string AXml_Rpc_Send(string aRPCRequest)
        {
            string cURL = "http://api.opensubtitles.org/xml-rpc";

            byte[] requestData = Encoding.ASCII.GetBytes(aRPCRequest);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Method = "POST";
            request.UserAgent = "OS Test User Agent";
            request.ContentType = "text/xml";
            request.Accept = "*/*";

            using (Stream requestStream = request.GetRequestStream())
                requestStream.Write(requestData, 0, requestData.Length);

            string result = null;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                        result = reader.ReadToEnd();
                }
            }
            return result;
        }

        public static string LocalizaImdb(string aImdb)
        {
            StringBuilder sb1 = new StringBuilder();

            sb1.Append("<methodCall>");
            sb1.Append("<methodName>GetIMDBMovieDetails</methodName>");
            sb1.Append("<params>");
            sb1.Append("<param>");
            sb1.Append("<value><string>" + aToken + "</string></value>");
            sb1.Append("</param>");
            sb1.Append("<param>");
            sb1.Append("<value><string>" + aImdb + "</string></value>");
            sb1.Append("</param>");
            sb1.Append("</params>");
            sb1.Append("</methodCall>");


            var result = AXml_Rpc_Send(sb1.ToString());


            return result;





        }

        public static string XML_RPC(String aRPCRequest)
        {
            var cURL = "http://api.opensubtitles.org/xml-rpc";

            byte[] requestData = Encoding.ASCII.GetBytes(aRPCRequest);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cURL);
            request.Method = "POST";
            request.UserAgent = "OS Test User Agent";
            request.ContentType = "text/xml";
            request.Accept = "*/*";

            using (Stream requestStream = request.GetRequestStream())
                requestStream.Write(requestData, 0, requestData.Length);

            string result = null;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                        result = reader.ReadToEnd();
                }
            }

            aToken = GetToken(result);

            return result;

        }


        public static string Login(string aUser, string aPass, string aLang)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version='1.0'?>");
            sb.Append("<methodCall>");
            sb.Append("<methodName>LogIn</methodName>");
            sb.Append("<params>");
            sb.Append("<param>");
            sb.Append("<value><string>" + aUser + "</string></value>");
            sb.Append("</param>");
            sb.Append("<param>");
            sb.Append("<value><string>" + aPass + "</string></value>");
            sb.Append("</param>");
            sb.Append("<param>");
            sb.Append("<value><string>" + aLang + "</string></value>");
            sb.Append("</param>");
            sb.Append("<param>");
            sb.Append("<value><string>FileBot v4.5.6</string></value>");
            sb.Append("</param>");
            sb.Append("</params>");
            sb.Append("</methodCall>");

            string aXml = sb.ToString();

            return XML_RPC(sb.ToString());

        }

        public static string SearchSubtitlesImdb(string aToken, Int16 aImdbId, string aSublanguageID)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version='1.0'?>");
            sb.Append("<methodCall>");
            sb.Append("  <methodName>SearchSubtitles</methodName>");
            sb.Append("  <params>");
            sb.Append("    <param>");
            sb.Append("      <value><string>{0}</string></value>");
            sb.Append("    </param>");
            sb.Append("  <param>");
            sb.Append("   <value>");
            sb.Append("    <array>");
            sb.Append("     <data>");
            sb.Append("      <value>");
            sb.Append("       <struct>");
            sb.Append("        <member>");
            sb.Append("         <name>sublanguageid</name>");
            sb.Append("         <value><string>{1}</string>");
            sb.Append("         </value>");
            sb.Append("        </member>");
            sb.Append("        <member>");
            sb.Append("         <name>imdbid</name>");
            sb.Append("         <value><string>{2}</string></value>");
            sb.Append("        </member>");
            sb.Append("       </struct>");
            sb.Append("      </value>");
            sb.Append("     </data>");
            sb.Append("    </array>");
            sb.Append("   </value>");
            sb.Append("  </param>");
            sb.Append(" </params>");
            sb.Append("</methodCall>");

            string nomeAprocurar = aImdbId.ToString();
            string result = string.Format(sb.ToString(), aToken, aSublanguageID, aImdbId);
            return AXml_Rpc_Send(result);
        }

        public static string GetToken(string axml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(axml);


            XmlNodeList elemList = doc.GetElementsByTagName("string");

            var nodes = elemList.Item(0).InnerText;


            return nodes;
        }
    }

}
