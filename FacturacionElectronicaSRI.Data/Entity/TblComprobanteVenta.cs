using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    [Index(nameof(IdEmpresa), Name = "IX_TblComprobanteVenta_IdEmpresa")]
    [Index(nameof(IdCliente), Name = "IX_TblComprobanteVenta_IdCliente")]
    public class TblComprobanteVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        [ForeignKey("IdEmpresa")]
        public TblEmpresa? Empresa { get; set; }

        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public TblCliente? Cliente { get; set; }

        public required DateTime FechaEmision { get; set; }

        public required string TipoComprobante { get; set; }

        public required string NumeroComprobante { get; set; } // Es el numero de comprobante que asigna el sistema cuyo formato es : 001-001-000000123 por ejemplo, ademas se debe hacer para que los ultimos 8 numeros sea incremental y rellenados con cero

        public required string FormaPago { get; set; } // Es la forma de pago que efectuara el cliente, estos se encuentran en la tabla de formas de pago del SRI

        public required decimal Subtotal0 { get; set; }

        public required decimal Subtotal12 { get; set; }

        public required decimal Descuento { get; set; }

        public required decimal Subtotal { get; set; }

        public required decimal TotalIva { get; set; }

        public required string DocSri { get; set; }
    }
}