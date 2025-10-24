using System.Net;

namespace FacturacionElectronica.Utility
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            this.Errors = new List<string>();
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public object? Result { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public List<string>? Errors { get; set; }
    }
}