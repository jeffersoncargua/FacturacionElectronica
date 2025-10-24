using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    public class TblTipoComprobante
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string TipoComprobante { get; set; }

        public required string Codigo { get; set; }
    }
}