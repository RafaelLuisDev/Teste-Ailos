namespace Questao5.Services.Models
{
    public class TransactionRequest
    {
        public Guid TransactionRequestId { get; set; }
        public Guid CurrentAccountId { get; set; }
        public char Type { get; set; }
        public double Value { get; set; }
    }
}
