namespace FacturacionElectronicaSRI.Repository
{
    public class ResponseRecepcionSRI
    {
        public string Estado { get; set; } = string.Empty;

        public string ClaveAcceso { get; set; } = string.Empty;
    }

    public class ResponseAutorizacionSRI
    {
        public string Estado { get; set; } = string.Empty;

        public string ClaveAcceso { get; set; } = string.Empty;

        public int Code { get; set; }
    }
}