using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    [Index(nameof(Ruc), Name = "IX_TblEmpresa_Ruc")]
    [Index(nameof(NombreComercial), Name = "IX_TblEmpresa_NombreComercial")]
    public class TblEmpresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string Ruc { get; set; }

        public required string RazonSocial { get; set; }

        public required string NombreComercial { get; set; }

        public required string DireccionMatriz { get; set; }

        public required string ObligadoLlevarContabilidad { get; set; }

        public required string Estado { get; set; }

        public int Ambiente { get; set; } // 1 es para ambiente de pruebas y 2 es para ambiente de produccion

        public string? PathCertificado { get; set; }

        public required string Contraseña { get; set; }

        public string? PathLogo { get; set; }

        // [DataType(DataType.EmailAddress)]
        public required string EmailEmpresa { get; set; }

        public required string TelefonoEmpresa { get; set; }

        public string? UrlRecepcionPrueba { get; set; }

        public string? UrlRecepcionProduccion { get; set; }

        public string? UrlAutorizacionPrueba { get; set; }

        public string? UrlAutorizacionProduccion { get; set; }
    }
}