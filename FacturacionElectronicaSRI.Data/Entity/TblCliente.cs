using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    [Index(nameof(IdEmpresa), Name = "IX_TblCliente_IdEmpresa")]
    [Index(nameof(Email), Name = "IX_TblCliente_Email")]
    public class TblCliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        public required string Nombres { get; set; }

        public required string Identificacion { get; set; }

        public required string Direccion { get; set; }

        public required string Telefono { get; set; }

        public required string Email { get; set; }
    }
}