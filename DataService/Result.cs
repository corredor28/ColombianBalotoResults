namespace DataService
{
    /// <summary>
    /// Model for a record as shown in http://www.baloto.com/filtro-historico-de-resultados.php
    /// </summary>
    public class Result
    {
        public int Year { get; set; }
        public int Id { get; set; }
        public string Number { get; set; }
        public bool IsPrize { get; set; }
        public bool IsSecondChancePrize { get; set; }
        public string SecondChanceNumber { get; set; }
        public string Date { get; set; }
    }
}
