using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Producto.DTO;

namespace FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO
{
    public class DetalleVentaDto
    {
        public int Id { get; set; }

        public int IdComprobanteVenta { get; set; }

        public ComprobanteVentaDto? ComprobanteVenta { get; set; } = null;

        public int IdProducto { get; set; }

        public ProductoDto? Producto { get; set; } = null!;

        public decimal Cantidad { get; set; } // Es la cantidad de productos o servicios vendidos

        public decimal PrecioUnitario { get; set; } // Es el valor unitario del producto o servicio vendido

        public required string Estado { get; set; } // Permite Verificar el estado de la venta que puede ser: abierta, cerrada, anulada, etc.

        public decimal Descuento { get; set; } // Permite verificar si existe un descuento en el producto

        public decimal VentaIva { get; set; } // Permite obtener el valor del iva de la venta del producto

        public decimal Total { get; set; } // Permite obtener el valor total de la venta del producto

        public required string Detalle { get; set; } // Permite mostarar el detalle con la talla en caso de venta
    }
}