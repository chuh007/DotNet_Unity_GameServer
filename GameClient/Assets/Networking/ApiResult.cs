namespace Networking
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T Data { get; private set; }
        public long StatusCode { get; private set; }
        public ApiError Error { get; private set; }

        public static ApiResult<T> Success(T data, long statusCode) => new ApiResult<T>
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };

        public static ApiResult<T> Failure(ApiError error, long statusCode) => new ApiResult<T>
        {
            IsSuccess = false,
            Error = error,
            StatusCode = statusCode
        };
    }
}