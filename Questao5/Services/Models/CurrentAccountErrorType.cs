using System.ComponentModel;

namespace Questao5.Services.Models
{
    public enum CurrentAccountErrorType
    {
        [Description("Conta corrente não cadastrada.")]
        INVALID_ACCOUNT,

        [Description("Conta corrente inativa. Apenas contas correntes ativas podem efetuar esta ação.")]
        INACTIVE_ACCOUNT,

        [Description("Valor inválido. Somente valores positivos são aceitos.")]
        INVALID_VALUE,
        
        [Description("Tipo de movimento inválido. Somente crédito (C) ou débito (D) são aceitos.")]
        INVALID_TYPE,
        
        [Description("Requisição já processada anteriormente.")]
        INVALID_REQUEST
    }
}
