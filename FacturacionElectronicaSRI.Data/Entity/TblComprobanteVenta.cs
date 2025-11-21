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

        public required decimal Subtotal0 { get; set; } // Por lo general su valor es cero 0

        public required decimal Subtotal15 { get; set; } // Es la suma de los valores de los productos + descuentos sin el iva o la base imponible considerando la suma de los productos + los descuentos

        public required decimal Descuento { get; set; } // Es el total la suma de todos los descuentos

        public required decimal TotalIva { get; set; } // Es la suma de todos los ivas de cada producto, o tambien es el iva de la base imponible que actualmente es del 15%

        // public required decimal ICE { get; set; } // Es el impuesto de consumos especiales (ICE)
        // public required decimal IVA15 { get; set; } // Es la suma de todos los ivas de cada producto
        public required decimal Propina { get; set; }

        public required decimal Total { get; set; } // Es el valor total de la venta que considera la base imponible + el totalIva15

        public string? DocSri { get; set; } = null;
    }
}