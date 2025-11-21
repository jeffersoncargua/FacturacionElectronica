using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML
{
    public class FimarArchivoXmlDto
    {
        [Required]
        [StringLength(maximumLength: 13, MinimumLength = 13)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Solo se permiten números")]
        public string EmpresaRuc { get; set; } = string.Empty;

        public IFormFile? EmpresaUbicacionArchivop12 { get; set; }

        [Required(ErrorMessage = "Campo requerido {0}")]
        public string? EmpresaContrasena { get; set; }

        [Required(ErrorMessage = "Campo requerido {0}")]
        public IFormFile? XmlDocumento { get; set; }

        [Required]
        [StringLength(maximumLength: 49, MinimumLength = 49)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Solo se permiten números")]
        public string ClaveAcceso { get; set; } = string.Empty;
    }

    public class ViewFirmaXmlDto
    {
        [Required(ErrorMessage = "Campo requerido {0}")]
        public string? XmlDocumentoFirmadoString { get; set; }

        [Required(ErrorMessage = "Campo requerido {0}")]
        public string? XmlDocumentoFirmado { get; set; }

        [Required]
        [StringLength(maximumLength: 49, MinimumLength = 49)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Solo se permiten números")]
        public string ClaveAcceso { get; set; } = string.Empty;

        public string? EmpresaCarpeta { get; set; }
    }

    public class ViewXmlDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public required string RucEmpresa { get; set; }

        public string? RutaXmlGenerado { get; set; }

        public string? RutaXmlFirmado { get; set; }

        public string? Mensaje { get; set; }

        public bool IsSuccess { get; set; } = false;
    }
}