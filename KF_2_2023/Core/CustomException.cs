using System.Net;

namespace OneTimeBuckAPI.Core
{
    public class CustomException : Exception
    {
        public HttpStatusCode statuscode { get; set; }
        public CustomException(string message, HttpStatusCode code) : base(message)
        {
            statuscode = code;
        }
    }
}
