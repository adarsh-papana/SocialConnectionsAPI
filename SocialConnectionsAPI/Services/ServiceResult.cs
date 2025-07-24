namespace SocialConnectionsAPI.Services
{
    public class ServiceResult
    {
        public bool IsSuccess { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public string ErrorCode { get; protected set; } // e.g., "user_not_found", "connection_exists"

        public static ServiceResult Success() => new ServiceResult { IsSuccess = true };
        public static ServiceResult Failure(string errorMessage, string errorCode = null) =>
            new ServiceResult { IsSuccess = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; private set; }

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { IsSuccess = true, Data = data };
        public static new ServiceResult<T> Failure(string errorMessage, string errorCode = null) =>
            new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage, ErrorCode = errorCode };
    }
}
