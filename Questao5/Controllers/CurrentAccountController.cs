using Microsoft.AspNetCore.Mvc;
using Questao5.Controllers.Models;
using Questao5.Services;
using Questao5.Services.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Questao5.Controllers
{
    /// <summary>
    /// Controlador de contas correntes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CurrentAccountController : ControllerBase
    {
        private readonly ICurrentAccountService currentAccountService;

        public CurrentAccountController(ICurrentAccountService currentAccountService)
        {
            this.currentAccountService = currentAccountService;
        }

        /// <summary>
        /// Retorna os dados da conta corrente
        /// </summary>
        /// <param name="id">Identificador único da conta corrente</param>
        /// <returns>Dados da conta corrente informada</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/CurrentAccount/3fa85f64-5717-4562-b3fc-2c963f66afa6
        ///
        /// </remarks>
        /// <response code="200">Retorna resposta padrão com os dados da conta corrente</response>
        /// <response code="400">Retorna resposta padrão com os dados do erro encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse> GetBalance(Guid id)
        {
            var result = currentAccountService.GetAccountBalance(id);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.Failure(result.ErrorType));
            }

            return Ok(ApiResponse.Success(result.Value));
        }

        /// <summary>
        /// Efetua uma movimentação em uma conta corrente
        /// </summary>
        /// <param name="transaction">Requisição da movimentação de uma conta corrente</param>
        /// <returns>Identificador único da movimentação efetuada</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/CurrentAccount/Transaction
        ///     {
        ///         "transactionRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "currentAccountId": "9d461ab5-8827-7451-c8ab-3d647a23bfd2",
        ///         "type": "C",
        ///         "value": 100
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Retorna resposta padrão com o identificador único da movimentação efetuada</response>
        /// <response code="400">Retorna resposta padrão com os dados do erro encontrado</response>

        [HttpPost("Transaction")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse> PostTransaction([FromBody] TransactionRequest transaction)
        {
            var result = currentAccountService.AddMoviment(transaction);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.Failure(result.ErrorType));
            }

            return Ok(ApiResponse.Success(result.Value));
        }
    }
}
