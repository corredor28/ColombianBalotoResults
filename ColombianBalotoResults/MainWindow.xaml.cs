using ColombianBalotoResults.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ColombianBalotoResults
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// List of result records
        /// </summary>
        private List<Result> results = new List<Result>();

        /// <summary>
        /// Performs main window startup tasks
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Clean labels;
            lblResult.Content = string.Empty;
            txbResults.Text = string.Empty;

            // Enable buttons if there is a results JSON file
            if (File.Exists(@"historic_data.json"))
            {
                btnCheck.IsEnabled = true;
                btnDuplicates.IsEnabled = true;
            }
        }
        
        /// <summary>
        /// Gets results since 2001 and saves them to a JSON file
        /// </summary>
        private void SaveResultsToJSON()
        {
            var resultBO = new ResultBO();
            
            // Get results list since 2001
            for (int i = 2000; i < DateTime.Now.Year; i++)
            {
                var year = Convert.ToString(i + 1);
                Updater uiUpdater = new Updater(UpdateYearLabel);
                Dispatcher.BeginInvoke(DispatcherPriority.Send, uiUpdater, year);
                results = resultBO.GetResultsByYear(year).ToList();
            }

            // Save result as a JSON file
            File.Delete(@"historic_data.json");
            using (FileStream fs = File.Open(@"historic_data.json", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, results);
            }
        }

        private delegate void Updater(string target);
        public void UpdateYearLabel(string year)
        {
            var labelText = FindResource("Label_Year").ToString();
            lblResult.Content = labelText + " " + year;
        }

        private delegate void Updater2();
        private void UpdateUI2()
        {
            btnCheck.IsEnabled = true;
            btnDuplicates.IsEnabled = true;
        }
        
        /// <summary>
        /// Gets the Baloto results from the official site
        /// </summary>
        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var syncEvent = new ManualResetEvent(false);
                var thread = new Thread(
                    () =>
                    {
                        SaveResultsToJSON();
                        syncEvent.Set();
                    }
                );
                thread.Start();

                Thread thread2 = new Thread(
                    () =>
                    {
                        syncEvent.WaitOne();
                        Updater2 uiUpdater = new Updater2(UpdateUI2);
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, uiUpdater);
                    }
                );
                thread2.Start();
            }
            catch (Exception ex)
            {
                lblResult.Content = ex.Message;
            }

            /*var progressHandler = new Progress<string>(value =>
            {
                label2.Text = value;
            });
            var progress = progressHandler as IProgress<string>;
            await Task.Run(() =>
            {
                for (int i = 0; i != 100; ++i)
                {
                    if (progress != null)
                        progress.Report("Stage " + i);
                    Thread.Sleep(100);
                }
            });
            label2.Text = "Completed.";*/
        }
        
        /// <summary>
        /// Checks if the typed number has been a result
        /// </summary>
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Read results from file
                results = null;
                using (var file = File.OpenText(@"historic_data.json"))
                {
                    var serializer = new JsonSerializer();
                    results = (List<Result>)serializer.Deserialize(file, typeof(List<Result>));
                }

                txbResults.Text = string.Empty;
                var typedNumber = String.Join("-", txtNumber.Text.Split('-').Select(n => n.Trim()).OrderBy(n => n));

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
                    
                    lblResult.Content = FindResource("String_IsResult").ToString();
                    var result = results.FirstOrDefault(n => n.Id == resultId);
                    var textNormal = FindResource("String_TypeNormal").ToString();
                    var textSecondChance = FindResource("String_TypeSecondChance").ToString();
                    var textYes = FindResource("String_Yes").ToString();
                    var textNo = FindResource("String_No").ToString();
                    txbResults.Text = String.Format(FindResource("String_CheckResult").ToString() // Date: {0}Type: {1}Winner: {2}
                                                    , result.Date + Environment.NewLine
                                                    , (isSecondChanceNumber ? textSecondChance : textNormal) + Environment.NewLine
                                                    , isWinner ? textYes : textNo
                                                    );
                }
                else
                {
                    lblResult.Content = FindResource("String_IsNotResult").ToString();
                }
            }
            catch (Exception ex)
            {
                lblResult.Content = ex.Message;
            }
        }

        /// <summary>
        /// Checks if there are duplicated numbers in the results database
        /// </summary>
        private void btnDuplicates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var normalList = results.Select(n => n.Number);
                var normalDuplicates = normalList.GroupBy(n => n).Where(g => g.Count() > 1).Select(gp => gp.Key);
                var secondChanceList = results.Select(n => n.SecondChanceNumber);
                var secondChanceDuplicates = secondChanceList.GroupBy(n => n).Where(g => g.Count() > 1).Select(gp => gp.Key);
                var totalList = normalList.Union(secondChanceList);
                var totalDuplicates = totalList.GroupBy(n => n).Where(g => g.Count() > 1).Select(gp => gp.Key);

                txbResults.Text = String.Format(FindResource("String_DuplicatesResult").ToString()
                    , (normalDuplicates.Any() ? String.Join(", ", normalDuplicates) : "0") + Environment.NewLine
                    , (secondChanceDuplicates.Any() ? String.Join(", ", secondChanceDuplicates) : "0") + Environment.NewLine
                    , (totalDuplicates.Any() ? String.Join(", ", totalDuplicates) : "0") + Environment.NewLine
                    );
            }
            catch (Exception ex)
            {
                lblResult.Content = ex.Message;
            }
        }
    }
}
