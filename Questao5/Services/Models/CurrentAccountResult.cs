namespace Questao5.Services.Models
{
    public class CurrentAccountResult<T>
    {
        public T? Value { get; }
        public bool IsSuccess { get; }
        public CurrentAccountErrorType? ErrorType { get; }

        private CurrentAccountResult(T value, bool isSuccess, CurrentAccountErrorType? error = null)
        {
            Value = value;
            IsSuccess = isSuccess;
            ErrorType = error;
        }

        public static CurrentAccountResult<T> Success(T value) => new(value, true);
        public static CurrentAccountResult<T> Failure(CurrentAccountErrorType error) => new(default, false, error);
    }
}
