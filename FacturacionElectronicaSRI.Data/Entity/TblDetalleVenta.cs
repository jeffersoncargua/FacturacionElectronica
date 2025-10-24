using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    public class TblDetalleVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdComprobanteVenta { get; set; }

        [ForeignKey("IdComprobanteVenta")]
        public TblComprobanteVenta? ComprobanteVenta { get; set; }

        public int IdProducto { get; set; }

        [ForeignKey("IdProducto")]
        public TblProductos? Producto { get; set; }

        public decimal Cantidad { get; set; } // Es la cantidad de productos o servicios vendidos

        public decimal PrecioUnitario { get; set; } // Es el valor unitario del producto o servicio vendido

        public required string Estado { get; set; } // Permite Verificar el estado de la venta que puede ser: abierta, cerrada, anulada, etc.

        public decimal Descuento { get; set; } // Permite verificar si existe un descuento en el producto

        public decimal VentaIva { get; set; } // Permite obtener el valor del iva de la venta del producto

        public decimal Total { get; set; } // Permite obtener el valor total de la venta del producto
    }
}