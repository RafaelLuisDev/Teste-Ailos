namespace Questao5.Services.Models
{
    public class AccountBalanceResponse
    {
        public int AccountNumber { get; set; }
        public string Name { get; set; }
        public DateTime QueryTimestamp { get; set; }
        public string Balance { get; set; }
    }
}
