using Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace DataAccess
{
    public class DataReader
    {
        private string appDataPath;

        /// <summary>
        /// Selects default culture
        /// </summary>
        public DataReader()
        {
            SetLanguageDictionary("en-US");
            var appPath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            appDataPath = Path.GetFullPath(Path.Combine(appPath, @"..\..\..\DataAccess\AppData"));
        }

        /// <summary>
        /// Sets resources culture - http://stackoverflow.com/a/35813707/1863970
        /// </summary>
        private void SetLanguageDictionary(string cultureName)
        {
            cultureName = cultureName ?? Thread.CurrentThread.CurrentCulture.ToString();
            switch (cultureName)
            {
                case "es-CO":
                    Resources.Culture = new System.Globalization.CultureInfo("es-CO");
                    break;
                default:
                    Resources.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
            }
        }
        
        /// <summary>
        /// Gets results since 2001 and saves them to a JSON file
        /// </summary>
        public string[] GetResults()
        {
            var resultBO = new ResultBO();
            var results = new List<Result>();
            var methodResult = new string[2];

            try
            {
                // Get results list since 2001
                for (int i = 2000; i < DateTime.Now.Year; i++)
                {
                    var year = Convert.ToString(i + 1);
                    results = resultBO.GetResultsByYear(year).ToList();
                }

                // Save result as a JSON file
                File.Delete(appDataPath + @"historic_data.json");
                using (FileStream fs = File.Open(appDataPath + @"\historic_data.json", FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, results);
                }
                methodResult[0] = Resources.Label_Done;
            }
            catch (Exception ex)
            {
                methodResult[0] = ex.Message;
            }

            return methodResult;
        }

        /// <summary>
        /// Checks if a given number has been a result
        /// </summary>
        public string[] CheckNumber(string number)
        {
            var results = new List<Result>();
            var methodResult = new string[2];

            try
            {
                // Check if file exists already
                /*if (!File.Exists(appDataPath + @"\historic_data.json"))
                {
                    GetResults();
                }*/

                // Read results from file
                using (var file = File.OpenText(appDataPath + @"\historic_data.json"))
                {
                    var serializer = new JsonSerializer();
                    results = (List<Result>)serializer.Deserialize(file, typeof(List<Result>));
                }

                methodResult[1] = string.Empty;
                var typedNumber = String.Join("-", number.Split('-').Select(n => n.Trim()).OrderBy(n => n));

                // Compare typed number against list
                var isNumber = false;
                var isSecondChanceNumber = false;
                var isWinner = false;
                int resultId = 0;
                foreach (var result in results)
                {
                    var resultNumber = String.Join("-", result.Number);
                    if (resultNumber == typedNumber)
                    {
                        // Number found
                        resultId = result.Id;
                        isWinner = result.IsPrize;
                        isNumber = true;
                        break;
                    }
                    var secondChanceNumber = String.Join("-", result.SecondChanceNumber);
                    if (secondChanceNumber == typedNumber)
                    {
                        // Second chance number found
                        resultId = result.Id;
                        isWinner = result.IsSecondChancePrize;
                        isNumber = true;
                        isSecondChanceNumber = true;
                        break;
                    }
                }

                if (isNumber)
                {
                    methodResult[0] = Resources.String_IsResult; //FindResource("String_IsResult").ToString();
                    var result = results.FirstOrDefault(n => n.Id == resultId);
                    var textNormal = Resources.String_TypeNormal;
                    var textSecondChance = Resources.String_TypeSecondChance;
                    var textYes = Resources.String_Yes;
                    var textNo = Resources.String_No;
                    methodResult[1] = String.Format(Resources.String_CheckResult // Date: {0}Type: {1}Winner: {2}
                                                    , result.Date + Environment.NewLine
                                                    , (isSecondChanceNumber ? textSecondChance : textNormal) + Environment.NewLine
                                                    , isWinner ? textYes : textNo
                                                    );
                }
                else
                {
                    methodResult[0] = Resources.String_IsNotResult;
                }
            }
            catch (Exception ex)
            {
                methodResult[0] = ex.Message;
            }

            return methodResult;
        }
    }
}