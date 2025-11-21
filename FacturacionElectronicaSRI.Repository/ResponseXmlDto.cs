namespace FacturacionElectronicaSRI.Repository
{
    public class ResponseXmlDto
    {
        public string? Message { get; set; } = null;
        public string? PathXML { get; set; } = null;
        public bool IsSuccess { get; set; } = false;
    }
}