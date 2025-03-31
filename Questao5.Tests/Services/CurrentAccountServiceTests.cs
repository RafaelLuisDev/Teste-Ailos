using Microsoft.Data.Sqlite;
using Moq;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;
using Questao5.Services;
using Questao5.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Questao5.Tests.Services
{
    public class CurrentAccountServiceTests
    {
        private readonly Mock<IDatabaseBootstrap> _mockDatabase;
        private readonly CurrentAccountService _service;

        public CurrentAccountServiceTests()
        {
            _mockDatabase = new Mock<IDatabaseBootstrap>();
            _service = new CurrentAccountService(_mockDatabase.Object);
        }

        [Theory]
        [MemberData(nameof(GetTransactionsData))]
        public void GetAccountBalance_ReturnsExpectedResults_WhenAccountConditionsSatisfied(List<Movimento> transactions, double expectedBalance)
        {
            // Arrange
            var account = new List<ContaCorrente>
                {
                    new ContaCorrente { Ativo = true }
                };
            _mockDatabase.Setup(db => db.ExecuteSelectQuery<ContaCorrente>(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(account);
            _mockDatabase.Setup(db => db.ExecuteSelectQuery<Movimento>(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(transactions);

            // Act
            var result = _service.GetAccountBalance(It.IsAny<Guid>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorType);
            Assert.Equal(expectedBalance.ToString("C", new CultureInfo("pt-BR")), result.Value.Balance);
        }

        [Theory]
        [InlineData(CurrentAccountErrorType.INVALID_ACCOUNT)]
        [InlineData(CurrentAccountErrorType.INACTIVE_ACCOUNT)]
        public void GetAccountBalance_ReturnsExpectedError_WhenAccountIsInvalidOrInactive(CurrentAccountErrorType expectedError)
        {
            // Arrange
            var account = new List<ContaCorrente>
                {
                    new ContaCorrente { Ativo = false }
                };
            _mockDatabase.Setup(db => db.ExecuteSelectQuery<ContaCorrente>(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(expectedError == CurrentAccountErrorType.INACTIVE_ACCOUNT ? account : new List<ContaCorrente>());

            // Act
            var result = _service.GetAccountBalance(It.IsAny<Guid>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedError, result.ErrorType);
        }

        [Fact]
        public void AddMoviment_ReturnsExpectedResults_WhenAllConditionsSatisfied()
        {
            // Arrange
            var account = new List<ContaCorrente>
                {
                    new ContaCorrente { Ativo = true }
                };
            _mockDatabase.Setup(db => db.ExecuteSelectQuery<ContaCorrente>(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(account);
            _mockDatabase.Setup(db => db.ExecuteQueries(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Returns(It.IsAny<List<int>>);

            // Act
            var result = _service.AddMoviment(new TransactionRequest()
            {
                TransactionRequestId = It.IsAny<Guid>(),
                CurrentAccountId = It.IsAny<Guid>(),
                Type = new Random().Next(0, 1) == 0 ? 'C' : 'D',
                Value = new Random().Next(1, int.MaxValue)
            });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorType);
            Assert.IsType<TransactionResponse>(result.Value);
            Assert.IsType<Guid>(result.Value.Id);
        }

        [Theory]
        [InlineData(CurrentAccountErrorType.INVALID_TYPE, 'B', 1)]
        [InlineData(CurrentAccountErrorType.INVALID_VALUE, 'C', 0)]
        public void AddMoviment_ReturnsExpectedError_WhenRequestConditionsUnsatisfied(CurrentAccountErrorType expectedError,
            char transactionType, double transactionValue)
        {
            // Arrange
            var transactionRequest = new TransactionRequest()
            {
                TransactionRequestId = It.IsAny<Guid>(),
                CurrentAccountId = It.IsAny<Guid>(),
                Type = transactionType,
                Value = transactionValue
            };

            // Act
            var result = _service.AddMoviment(transactionRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedError, result.ErrorType);
            Assert.Null(result.Value);
        }

        [Theory]
        [MemberData(nameof(AddMovimentErrorData))]
        public void AddMoviment_ReturnsExpectedError_WhenDatabaseReturnsFailure(CurrentAccountErrorType expectedError,
            List<ContaCorrente> account)
        {
            // Arrange
            var transactionRequest = new TransactionRequest()
            {
                TransactionRequestId = It.IsAny<Guid>(),
                CurrentAccountId = It.IsAny<Guid>(),
                Type = new Random().Next(0, 1) == 0 ? 'C' : 'D',
                Value = new Random().Next(1, int.MaxValue)
            };

            _mockDatabase.Setup(db => db.ExecuteSelectQuery<ContaCorrente>(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(account);
            _mockDatabase.Setup(db => db.ExecuteQueries(It.IsAny<List<KeyValuePair<string, object>>>()))
                .Throws(It.IsAny<Exception>);

            // Act
            var result = _service.AddMoviment(transactionRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedError, result.ErrorType);
            Assert.Null(result.Value);
        }

        public static IEnumerable<object[]> GetTransactionsData()
        {
            yield return new object[]
            {
                new List<Movimento>(),
                0
            };

            yield return new object[]
            {
                new List<Movimento>
                {
                    new Movimento { TipoMovimento = 'C', Valor = 100 },
                    new Movimento { TipoMovimento = 'D', Valor = 50 }
                },
                50
            };

            yield return new object[]
            {
                new List<Movimento>
                {
                    new Movimento { TipoMovimento = 'C', Valor = 100 },
                    new Movimento { TipoMovimento = 'D', Valor = 200 }
                },
                -100
            };
        }

        public static IEnumerable<object[]> AddMovimentErrorData()
        {
            yield return new object[]
            {
                CurrentAccountErrorType.INVALID_ACCOUNT,
                new List<ContaCorrente>()
            };

            yield return new object[]
            {
                CurrentAccountErrorType.INACTIVE_ACCOUNT,
                new List<ContaCorrente>()
                {
                    new ContaCorrente { Ativo = false }
                }
            };

            yield return new object[]
            {
                CurrentAccountErrorType.INVALID_REQUEST,
                new List<ContaCorrente>()
                {
                    new ContaCorrente { Ativo = true }
                }
            };
        }

    }
}
