using System.Net;

namespace OneTimeBuckAPI.Core
{
    public class CustomResponse
    {
        public static CustomResponse Create(HttpStatusCode _code, Object? _result = null, string? _err = null, bool _bodyonly = false)
        {
            return new CustomResponse(_code, _result, _err, _bodyonly);
        }

        public HttpStatusCode status { get; set; }
        public Object? result { get; set; }
        public string? errormessage { get; set; }
        public bool bodyonly { get; set; }

        protected CustomResponse(HttpStatusCode _status, Object? _result = null, string? _err = null, bool _bodyonly = false)
        {
            status = _status;
            result = _result;
            errormessage = _err;
            bodyonly = _bodyonly;
        }

    }
}
