using System.ComponentModel.DataAnnotations;

namespace FacturacionElectronicaSRI.Data.Model.Empresa.DTO
{
    public class EmpresaViewDto
    {
        public int Id { get; set; }

        public required string Ruc { get; set; }

        public required string RazonSocial { get; set; }

        public required string NombreComercial { get; set; }

        public required string DireccionMatriz { get; set; }

        public required string ObligadoLlevarContabilidad { get; set; }

        public required string Estado { get; set; }

        public int Ambiente { get; set; }

        public string? PathCertificado { get; set; } = null;

        public required string Contraseña { get; set; }

        public string? PathLogo { get; set; } = null;

        [DataType(DataType.EmailAddress)]
        public required string EmailEmpresa { get; set; }

        public required string TelefonoEmpresa { get; set; }

        public string? UrlRecepcionPrueba { get; set; }

        public string? UrlRecepcionProduccion { get; set; }

        public string? UrlAutorizacionPrueba { get; set; }

        public string? UrlAutorizacionProduccion { get; set; }
    }
}