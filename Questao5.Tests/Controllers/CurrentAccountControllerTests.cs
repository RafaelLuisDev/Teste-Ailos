using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Questao5.Controllers;
using Questao5.Controllers.Models;
using Questao5.Services;
using Questao5.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Questao5.Tests.Controllers
{
    public class CurrentAccountControllerTests
    {
        private readonly Mock<ICurrentAccountService> _mockService;
        private readonly CurrentAccountController _controller;

        public CurrentAccountControllerTests()
        {
            _mockService = new Mock<ICurrentAccountService>();
            _controller = new CurrentAccountController(_mockService.Object);
        }

        [Fact]
        public void GetBalance_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var expectedBalance = "R$ 4,00";
            var serviceResult = CurrentAccountResult<AccountBalanceResponse>.Success(new() { Balance = expectedBalance });
            _mockService.Setup(s => s.GetAccountBalance(It.IsAny<Guid>())).Returns(serviceResult);

            // Act
            var result = _controller.GetBalance(Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal("SUCCESS", apiResponse.Status);

            var accountBalanceResponse = Assert.IsType<AccountBalanceResponse>(apiResponse.Data);
            Assert.Equal(expectedBalance, accountBalanceResponse.Balance);
        }

        [Theory]
        [InlineData(CurrentAccountErrorType.INVALID_ACCOUNT)]
        [InlineData(CurrentAccountErrorType.INACTIVE_ACCOUNT)]
        public void GetBalance_ReturnsBadRequest_WhenServiceReturnsFailure(CurrentAccountErrorType errorType)
        {
            // Arrange
            var serviceResult = CurrentAccountResult<AccountBalanceResponse>.Failure(errorType);
            _mockService.Setup(s => s.GetAccountBalance(It.IsAny<Guid>())).Returns(serviceResult);

            // Act
            var result = _controller.GetBalance(Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal("ERROR", apiResponse.Status);
            Assert.NotNull(apiResponse.Message);
            Assert.Equal(errorType.ToString(), apiResponse.ErrorType);
        }

        [Fact]
        public void PostTransaction_ReturnsOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var serviceResult = CurrentAccountResult<TransactionResponse>.Success(new() { Id = expectedId });
            _mockService.Setup(s => s.AddMoviment(It.IsAny<TransactionRequest>())).Returns(serviceResult);

            // Act
            var result = _controller.PostTransaction(It.IsAny<TransactionRequest>());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal("SUCCESS", apiResponse.Status);

            var transactionResponse = Assert.IsType<TransactionResponse>(apiResponse.Data);
            Assert.Equal(expectedId, transactionResponse.Id);
        }

        [Theory]
        [InlineData(CurrentAccountErrorType.INVALID_ACCOUNT)]
        [InlineData(CurrentAccountErrorType.INACTIVE_ACCOUNT)]
        [InlineData(CurrentAccountErrorType.INVALID_VALUE)]
        [InlineData(CurrentAccountErrorType.INVALID_TYPE)]
        [InlineData(CurrentAccountErrorType.INVALID_REQUEST)]
        public void PostTransaction_ReturnsBadRequest_WhenServiceReturnsFailure(CurrentAccountErrorType errorType)
        {
            // Arrange
            var serviceResult = CurrentAccountResult<TransactionResponse>.Failure(errorType);
            _mockService.Setup(s => s.AddMoviment(It.IsAny<TransactionRequest>())).Returns(serviceResult);

            // Act
            var result = _controller.PostTransaction(It.IsAny<TransactionRequest>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal("ERROR", apiResponse.Status);
            Assert.NotNull(apiResponse.Message);
            Assert.Equal(errorType.ToString(), apiResponse.ErrorType);
        }
    }
}
