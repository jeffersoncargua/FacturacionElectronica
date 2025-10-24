using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    public class TblTipoIdentificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string TipoIdentificacion { get; set; }

        public required string Codigo { get; set; }
    }
}