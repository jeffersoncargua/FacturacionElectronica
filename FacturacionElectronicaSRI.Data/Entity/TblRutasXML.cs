using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    public class TblRutasXML
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        public TblEmpresa? Empresa { get; set; }

        public string? RutaGenerados { get; set; }

        public string? RutaFirmados { get; set; }

        public string? RutaAutorizados { get; set; }

        public string? ClaveAcceso { get; set; }

        public string? EstadoRecepcion { get; set; }

        public string? PathXMLPDF { get; set; }
    }
}