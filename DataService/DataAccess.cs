using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DataService
{
    public class DataAccess
    {
        /// <summary>
        /// Checks if a given number has been a result
        /// </summary>
        public string[] CheckNumber(string number)
        {
            var results = new List<Result>();
            var methodResult = new string[2];
            try
            {
                // Read results from file
                using (var file = File.OpenText(@"historic_data.json"))
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
                    methodResult[0] = FindResource("String_IsResult").ToString();
                    var result = results.FirstOrDefault(n => n.Id == resultId);
                    var textNormal = FindResource("String_TypeNormal").ToString();
                    var textSecondChance = FindResource("String_TypeSecondChance").ToString();
                    var textYes = FindResource("String_Yes").ToString();
                    var textNo = FindResource("String_No").ToString();
                    methodResult[1] = String.Format(FindResource("String_CheckResult").ToString() // Date: {0}Type: {1}Winner: {2}
                                                    , result.Date + Environment.NewLine
                                                    , (isSecondChanceNumber ? textSecondChance : textNormal) + Environment.NewLine
                                                    , isWinner ? textYes : textNo
                                                    );
                }
                else
                {
                    methodResult[0] = FindResource("String_IsNotResult").ToString();
                }
            }
            catch (Exception ex)
            {
                methodResult[0] = ex.Message;
            }
            return methodResult;
        }

        private object FindResource(string v)
        {
            throw new NotImplementedException();
        }
    }
}