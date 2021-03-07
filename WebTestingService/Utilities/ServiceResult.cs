
namespace WebTestingService.Utilities
{
    public class ServiceResult
    {
        public bool Succeeded { get; }
        public string ErrorMessage { get; }
        public int ErrorCode { get; }

        public ServiceResult(bool succeeded = true)
        {
            Succeeded = succeeded;
        }

        public ServiceResult(bool succeeded, int errCode, string errMsg)
        {
            Succeeded = succeeded;
            ErrorMessage = errMsg;
            ErrorCode = errCode;
        }

        public ServiceResult(int errCode, string errMsg)
        {
            Succeeded = false;
            ErrorMessage = errMsg;
            ErrorCode = errCode;
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; }

        public ServiceResult(T data) : base()
        {
            Data = data;
        }

        public ServiceResult(bool succeeded, int errCode, string errMsg, T data) : base(succeeded, errCode, errMsg)
        {
            Data = data;
        }

        public ServiceResult(int errCode, string errMsg, T data) : base(errCode, errMsg)
        {
            Data = data;
        }
    }
}
