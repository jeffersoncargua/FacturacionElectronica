namespace FacturacionElectronicaSRI.Data.Model.Venta.DTO
{
    public class ShoppingCartDto
    {
        public int ProductoId { get; set; }
        public string? PathImagen { get; set; }
        public required string CodigoPrincipal { get; set; } // Es el codigo principal para la busqueda
        public int Cantidad { get; set; }
        public required string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; } // Es un valor en porcentaje que se da a un producto para que tenga menor valor del precio unitario
        public decimal VentaIva { get; set; } // Es el iva que se le cobra al producto
        public decimal Total { get; set; } // Es el valor total del producto de la suma del valor unitario por la cantidad y el descuento
    }
}