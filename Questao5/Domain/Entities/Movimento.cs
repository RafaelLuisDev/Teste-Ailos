namespace Questao5.Domain.Entities
{
    public class Movimento
    {
        public string IdMovimento { get; set; }
        public string IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; }
        public double Valor { get; set; }
    }
}
