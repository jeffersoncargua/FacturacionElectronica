using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;

namespace FacturacionElectronicaSRI.Data.Model.Venta.DTO
{
    public class RutasFacturacionDto
    {
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        // public EmpresaDto? Empresa { get; set; }
        public EmpresaViewDto? Empresa { get; set; }

        public string? RutaGenerados { get; set; } = null; // Es la ruta donde se guardan xml generados

        public string? RutaFirmados { get; set; } = null; // Es la ruta donde se guardan los xml firmados

        public string? RutaAutorizados { get; set; } = null; // Es la ruta donde se guardan los xml autorizados

        public string? ClaveAcceso { get; set; }

        public string? EstadoRecepcion { get; set; }

        public string? PathXMLPDF { get; set; } = null; // Es la ruta donde se guardan los PDFs de la Factura
    }
}