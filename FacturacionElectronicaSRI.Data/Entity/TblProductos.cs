using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    [Index(nameof(CodigoPrincipal), Name = "IX_TblProductos_CodigoPrincipal")]
    public class TblProductos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? PathImagen { get; set; }

        public required string CodigoPrincipal { get; set; }

        public string? CodigoAuxiliar { get; set; }

        public required string Descripcion { get; set; }

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        public string? Estado { get; set; }

        public decimal? Descuento { get; set; } = 0.00M;
    }
}