namespace FacturacionElectronicaSRI.Data.Model.Producto.DTO
{
    public class ProductoDto
    {
        public int Id { get; set; }

        public required string CodigoPrincipal { get; set; }

        public string? CodigoAuxiliar { get; set; }

        public required string Descripcion { get; set; }

        public decimal PrecioUnitario { get; set; }

        public int Cantidad { get; set; }

        public string? Estado { get; set; }
    }
}