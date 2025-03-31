using Questao5.Services.Models;

namespace Questao5.Services
{
    public interface ICurrentAccountService
    {
        CurrentAccountResult<AccountBalanceResponse> GetAccountBalance(Guid id);
        CurrentAccountResult<TransactionResponse> AddMoviment(TransactionRequest moviment);
    }
}
