namespace FacturacionElectronicaSRI.Data.Model.Venta.DTO
{
    public class VentaDto
    {
        public required string IdentificacionCliente { get; set; }

        public required string ShoppingCart { get; set; }

        public required string RucEmpresa { get; set; }

        public required decimal Descuento { get; set; } // Es la suma de los descuentos de los productos

        public required decimal Total { get; set; } // Suma de los productos, del iva y del descuento

        public required decimal SubTotal15 { get; set; } // Este es el subtotal sin el iva de los productos + el descuento

        public required decimal TotalIVA { get; set; } // Este es el IVA Total de la venta

        public required string Token { get; set; } // Este es el token del pago con kushki

        public required bool IsDeferred { get; set; } = false; // Este paremetro sirve para saber si el pago es diferido o no

        public int Plazos { get; set; } = 0; // Este parametro sirve en el caso de que se realice un pago en plazos

        public required string FormaPago { get; set; } // Este parametro permitira recibir la forma de pago como lo va a realizar el cliente
    }
}