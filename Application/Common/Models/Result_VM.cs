namespace Application.Common.Models
{
    public class Result_VM<T> : BaseResult
    {
        public T? Result { get; set; }
    }
    public class BaseResult
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}
