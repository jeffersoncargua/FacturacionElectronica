namespace FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO
{
    public class ComprobanteVentaDto
    {
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        public int IdCliente { get; set; }

        public required DateTime FechaEmision { get; set; }

        public required string TipoComprobante { get; set; }

        public required string FormaPago { get; set; } // Es la forma de pago que efectuara el cliente, estos se encuentran en la tabla de formas de pago del SRI

        public required decimal Subtotal0 { get; set; }

        public required decimal Subtotal12 { get; set; }

        public required decimal Descuento { get; set; }

        public required decimal Subtotal { get; set; }

        public required decimal TotalIva { get; set; }

        public required string DocSri { get; set; }
    }
}