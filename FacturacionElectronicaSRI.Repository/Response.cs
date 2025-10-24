using System.Net;

namespace FacturacionElectronicaSRI.Repository
{
    public class Response
    {
        public Response()
        {
            Errors = new List<string>();
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public object? Result { get; set; } = null;

        public HttpStatusCode StatusCode { get; set; }

        public List<string>? Errors { get; set; }
    }
}