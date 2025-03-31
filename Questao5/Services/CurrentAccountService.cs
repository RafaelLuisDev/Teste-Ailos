using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;
using Questao5.Services.Models;
using System.Globalization;

namespace Questao5.Services
{
    public class CurrentAccountService : ICurrentAccountService
    {
        private readonly IDatabaseBootstrap database;

        public CurrentAccountService(IDatabaseBootstrap database)
        {
            this.database = database;
        }
        public CurrentAccountResult<AccountBalanceResponse> GetAccountBalance(Guid id)
        {
            double balance = 0;

            string accountSelectQuery = $"SELECT * FROM contacorrente WHERE idcontacorrente = @CurrentAccountId";
            var account = database.ExecuteSelectQuery<ContaCorrente>(accountSelectQuery, new { CurrentAccountId = id });
            if (!account.Any())
                return CurrentAccountResult<AccountBalanceResponse>.Failure(CurrentAccountErrorType.INVALID_ACCOUNT);
            else
            {
                if (!account.First().Ativo)
                    return CurrentAccountResult<AccountBalanceResponse>.Failure(CurrentAccountErrorType.INACTIVE_ACCOUNT);

                string transactionSelectQuery = $"SELECT * FROM movimento WHERE idcontacorrente = @CurrentAccountId";
                var accountTransactions = database.ExecuteSelectQuery<Movimento>(transactionSelectQuery, new { CurrentAccountId = id });
                if (accountTransactions.Any())
                {
                    var creditTransactions = accountTransactions.Where(x => x.TipoMovimento == 'C').Sum(x => x.Valor);
                    var debitTransations = accountTransactions.Where(x => x.TipoMovimento == 'D').Sum(x => x.Valor);
                    balance = creditTransactions - debitTransations;
                }
            }

            return CurrentAccountResult<AccountBalanceResponse>.Success(new AccountBalanceResponse()
            {
                AccountNumber = account.First().Numero,
                Name = account.First().Nome,
                QueryTimestamp = DateTime.Now,
                Balance = balance.ToString("C", new CultureInfo("pt-BR"))
            });
        }

        public CurrentAccountResult<TransactionResponse> AddMoviment(TransactionRequest request)
        {
            if (request.Value <= 0)
                return CurrentAccountResult<TransactionResponse>.Failure(CurrentAccountErrorType.INVALID_VALUE);
            if (request.Type != 'C' && request.Type != 'D')
                return CurrentAccountResult<TransactionResponse>.Failure(CurrentAccountErrorType.INVALID_TYPE);

            string accountSelectQuery = $"SELECT * FROM contacorrente WHERE idcontacorrente = @CurrentAccountId";
            var account = database.ExecuteSelectQuery<ContaCorrente>(accountSelectQuery, new { CurrentAccountId = request.CurrentAccountId });
            if (!account.Any())
                return CurrentAccountResult<TransactionResponse>.Failure(CurrentAccountErrorType.INVALID_ACCOUNT);
            else
            {
                if (!account.First().Ativo)
                    return CurrentAccountResult<TransactionResponse>.Failure(CurrentAccountErrorType.INACTIVE_ACCOUNT);

                var transactionId = Guid.NewGuid();

                string transactionInsertQuery =
                    @$"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) 
                        VALUES (@TransactionId, @CurrentAccountId, @TransactionDate, @Type, @Value)";
                var transactionParams = new
                {
                    TransactionId = transactionId,
                    CurrentAccountId = request.CurrentAccountId,
                    TransactionDate = DateTime.Now,
                    Type = request.Type,
                    Value = request.Value
                };

                string idempotenceInsertQuery =
                    @$"INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) 
                        VALUES (@IdempotenceKey, @Request, @Result)";
                var idempotenceParams = new
                {
                    IdempotenceKey = request.TransactionRequestId,
                    Request = JsonConvert.SerializeObject(request),
                    Result = transactionId
                };

                try
                {
                    database.ExecuteQueries(new List<KeyValuePair<string, object>>()
                    {
                        new KeyValuePair<string, object>(transactionInsertQuery, transactionParams),
                        new KeyValuePair<string, object>(idempotenceInsertQuery, idempotenceParams)
                    });
                }
                catch (Exception)
                {
                    return CurrentAccountResult<TransactionResponse>.Failure(CurrentAccountErrorType.INVALID_REQUEST);
                }

                return CurrentAccountResult<TransactionResponse>.Success(new TransactionResponse() { Id = transactionId });
            }
        }
    }
}
