using Questao5.Services.Models;

namespace Questao5.Controllers.Models
{
    public class ApiResponse
    {
        public string Status { get; }
        public string Message { get; }
        public object Data { get; }
        public string ErrorType { get; }

        private ApiResponse(StatusResponse status, object data = null, CurrentAccountErrorType? error = null)
        {
            Status = status.ToString();
            Data = data;
            ErrorType = error?.ToString();
            Message = error?.GetDescription();
        }

        public static ApiResponse Success(object data) => new(StatusResponse.SUCCESS, data);
        public static ApiResponse Failure(CurrentAccountErrorType? error) => new(StatusResponse.ERROR, error: error);
    }
}
