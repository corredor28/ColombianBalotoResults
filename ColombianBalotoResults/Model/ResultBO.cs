using ColombianBalotoResults.Helpers;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace ColombianBalotoResults.Model
{
    public class ResultBO
    {
        /// <summary>
        /// Gets all results for a given year
        /// </summary>
        public IEnumerable<Result> GetResultsByYear(string year)
        {
            var results = new List<Result>();

            try
            {
                // Prepare POST request to Baloto server
                string url = "http://www.baloto.com/administrator/index.php/ajax/results";
                StringBuilder sBuilder = new StringBuilder();

                // Add year parameter to query
                Constants.AppendParameter(sBuilder, "field_a_o_value", year);

                byte[] byteArray = Encoding.UTF8.GetBytes(sBuilder.ToString());
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2288.6 Safari/537.36";
                request.ProtocolVersion = HttpVersion.Version11;
                
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                    requestStream.Close();
                }

                // Perform request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();

                // Parse html stream into a searchable object with HTML Agility Pack
                var document = new HtmlDocument();
                document.Load(stream);
                var page = document.DocumentNode;

                // Find the results container table with Fizzler Agility Pack
                var tbody = page.QuerySelectorAll("tbody").First();

                // Declare TR tag counter 
                var trCount = 0;
                var yearNumber = Convert.ToInt32(year);

                // Loop through result table rows
                foreach (var tr in tbody.QuerySelectorAll("tr"))
                {
                    // Ignore first two TR tags (table rows)
                    if (trCount > 1)
                    {
                        var tds = tr.QuerySelectorAll("td");
                        var tdTotal = tds.Count();
                        // Check if this row is relevant
                        if (tdTotal == 6)
                        {
                            var result = new Result();
                            result.Year = yearNumber;

                            var tdArray = tds.ToArray();
                            // Save Id
                            result.Id = Convert.ToInt32(tdArray[0].InnerText);
                            // Save number
                            var arrCifras = tdArray[1].InnerText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                            result.Number = String.Join("-", arrCifras);
                            // Check if its prize
                            result.IsPrize = tdArray[2].InnerText.Trim() != string.Empty;
                            // Check if its second chance prize
                            result.IsSecondChancePrize = tdArray[3].InnerText.Trim() != string.Empty;
                            // Save second chance number
                            var arrCifrasRevancha = tdArray[4].InnerText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                            result.SecondChanceNumber = String.Join("-", arrCifrasRevancha);
                            // Save draw date
                            result.Date = tdArray[5].InnerText;
                            results.Add(result);
                        }
                    }
                    trCount++;
                }
            }
            catch (Exception)
            {
                //lblResult.Content = ex.Message;
                // Log error
                return results;
            }

            return results;
        }
    }
}
