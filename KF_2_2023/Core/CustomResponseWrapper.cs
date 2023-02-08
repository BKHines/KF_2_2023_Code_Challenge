using System.Net;
using System.Text.Json;

namespace OneTimeBuckAPI.Core
{
    public class CustomResponseWrapper
    {
        private readonly RequestDelegate _next;
        private readonly CustomLogger _logger;

        public CustomResponseWrapper(RequestDelegate next, ILogger<CustomResponseWrapper> logger)
        {
            _next = next;
            _logger = new CustomLogger(logger);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request != null && context.Request.Method.Equals("options", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _next(context);
                }
                else
                {
                    var originBody = context.Response.Body;
                    try
                    {
                        var memStream = new MemoryStream();
                        context.Response.Body = memStream;

                        //context.Response.Headers = context.Request.Headers;

                        await _next(context).ConfigureAwait(false);

                        memStream.Position = 0;
                        var responseBody = new StreamReader(memStream).ReadToEnd();

                        var formattedResp = GetFormattedResponse(context, responseBody);

                        if (formattedResp.bodyonly)
                        {
                            responseBody = formattedResp.errormessage;
                        }
                        else
                        {
                            formattedResp.errormessage = !string.IsNullOrWhiteSpace(formattedResp.errormessage) ? formattedResp.errormessage.Replace("\"", "") : string.Empty;
                            //Custom logic to modify response
                            responseBody = String.IsNullOrWhiteSpace(responseBody) ? JsonSerializer.Serialize(formattedResp) : responseBody.Replace(responseBody, JsonSerializer.Serialize(formattedResp), StringComparison.InvariantCultureIgnoreCase);
                        }

                        //_logger.LogMessage(LogType.info, new CustomLog() { messages = new List<string>() { $"response body: {responseBody}" }, title = "Response Body" }, null);

                        var memoryStreamModified = new MemoryStream();
                        var sw = new StreamWriter(memoryStreamModified);
                        sw.Write(responseBody);
                        sw.Flush();
                        memoryStreamModified.Position = 0;

                        await memoryStreamModified.CopyToAsync(originBody).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        _logger.LogMessage(LogType.info, new CustomLog() { messages = new List<string>() { $"CRW Error: {msg}" }, title = "Response Body" }, ex);
                    }
                    finally
                    {
                        context.Response.Body = originBody;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                _logger.LogMessage(LogType.info, new CustomLog() { messages = new List<string>() { $"CRW Error: {msg}" }, title = "Response Body" }, ex);
            }
        }

        private CustomResponse GetFormattedResponse(HttpContext context, string body)
        {
            CustomResponse eResponse;
            switch (context.Response.StatusCode)
            {
                case (int)HttpStatusCode.OK:
                    {
                        var _bodyObject = body == null ? null : JsonSerializer.Deserialize<object>(body);
                        eResponse = CustomResponse.Create(HttpStatusCode.OK, _result: _bodyObject, string.Empty);
                    }
                    break;
                case (int)HttpStatusCode.Forbidden:
                    {
                        var errorMessage = "You are not authorized to request this content.";
                        eResponse = CustomResponse.Create(HttpStatusCode.Forbidden, null, errorMessage);
                    }
                    break;
                case (int)HttpStatusCode.BadRequest:
                    {
                        var errorMessage = "Your request was not structured correctly and the error has been logged.";
                        eResponse = CustomResponse.Create(HttpStatusCode.BadRequest, null, errorMessage);
                    }
                    break;
                case (int)HttpStatusCode.Unauthorized:
                    {
                        var errorMessage = "Your request was not authorized and the error has been logged.";
                        eResponse = CustomResponse.Create(HttpStatusCode.BadRequest, null, errorMessage);
                    }
                    break;
                case (int)HttpStatusCode.ExpectationFailed:
                    {
                        eResponse = CustomResponse.Create(HttpStatusCode.ExpectationFailed, null, body);
                    }
                    break;
                case (int)HttpStatusCode.Ambiguous:
                    {
                        eResponse = CustomResponse.Create(HttpStatusCode.Ambiguous, null, body);
                    }
                    break;
                case (int)HttpStatusCode.Conflict:
                    {
                        eResponse = CustomResponse.Create(HttpStatusCode.Conflict, null, body, true);
                    }
                    break;
                case (int)HttpStatusCode.Accepted:
                    {
                        eResponse = CustomResponse.Create(HttpStatusCode.Accepted, null, body, true);
                    }
                    break;
                case (int)HttpStatusCode.PartialContent:
                    {
                        eResponse = CustomResponse.Create(HttpStatusCode.PartialContent, null, body, true);
                    }
                    break;
                default:
                    {
                        var errorMessage = "An error occurred and has been logged.";
                        eResponse = CustomResponse.Create(HttpStatusCode.InternalServerError, null, errorMessage);
                    }
                    break;
            }

            return eResponse;
        }

    }
}
