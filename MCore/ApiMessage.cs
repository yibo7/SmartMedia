namespace SmartMedia.MCore
{
    public class ApiMessage<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int code { get; set; }
        public bool Success { get; set; }
    }
}
