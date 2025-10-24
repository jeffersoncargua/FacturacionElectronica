using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    public class TblCliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        public TblEmpresa? Empresa { get; set; }

        public required string Nombres { get; set; }

        public required string Identificacion { get; set; }

        public required string Direccion { get; set; }

        public required string Telefono { get; set; }

        public required string Email { get; set; }
    }
}