namespace FacturacionElectronicaSRI.Data.Model.Cliente.DTO
{
    public class ClienteDto
    {
        public int Id { get; set; }

        public int IdEmpresa { get; set; }

        public required string Nombres { get; set; }

        public required string Identificacion { get; set; }

        public required string Direccion { get; set; }

        public required string Telefono { get; set; }

        public required string Email { get; set; }
    }
}