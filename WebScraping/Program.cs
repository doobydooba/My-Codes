using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping
{
    class Program
    {

        static void Main(string[] args)
        {
            var webClient = new WebClient();
            var html = webClient.DownloadString("http://mistersnacks.com/product-category/002-trail-mixes/");

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            //Console.WriteLine(htmlDocument.DocumentNode.InnerHtml);

            var nodes =
                htmlDocument
                .DocumentNode
                .Descendants("div")
            .Where(node =>
                node.Attributes["class"] != null &&
                node.Attributes["class"].Value.Contains("product__title"));

            foreach (var node in nodes)
            {
                Console.WriteLine(node.InnerText);

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScraperConnection"].ConnectionString))
                {
                    con.Open();

                    var cmd = con.CreateCommand();
                    cmd.CommandText = "dbo.Scraper_Insert";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", node.InnerText);

                    SqlParameter idParam = cmd.Parameters.Add("@Id", SqlDbType.Int);
                    idParam.Direction = System.Data.ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                }
                Console.ReadLine();
            }


        }
    }
}
