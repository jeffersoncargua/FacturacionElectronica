using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "infoFactura")]
    public class InformacionFactura
    {
        [XmlElement(ElementName = "fechaEmision")]
        public required string FechaEmision { get; set; } // Formato DD/MM/AAAA

        [XmlElement(ElementName = "dirEstablecimiento")]
        public required string DirEstablecimiento { get; set; }

        [XmlElement(ElementName = "contribuyenteEspecial")]
        public string? ContribuyenteEspecial { get; set; } // Opcional

        [XmlElement(ElementName = "obligadoContabilidad")]
        public required string ObligadoContabilidad { get; set; } // Puede ser SI o NO

        [XmlElement(ElementName = "tipoIdentificacionComprador")]
        public required string TipoIdentificacionComprador { get; set; } // Puede ser 05-Cedula, 04-RUC, 06-Pasaporte, 07-Consumidor Final

        [XmlElement(ElementName = "razonSocialComprador")]
        public required string RazonSocialComprador { get; set; } // Puede llevar el nombre del comprador

        [XmlElement(ElementName = "identificacionComprador")]
        public required string IdentificacionComprador { get; set; }

        [XmlElement(ElementName = "direccionComprador")]
        public required string DireccionComprador { get; set; }

        [XmlElement(ElementName = "totalSinImpuestos")]
        public required decimal TotalSinImpuestos { get; set; }

        [XmlElement(ElementName = "totalDescuento")]
        public required decimal TotalDescuento { get; set; }

        [XmlElement(ElementName = "totalConImpuestos")]
        public required List<TotalImpuesto>? TotalConImpuestos { get; set; }

        [XmlElement(ElementName = "propina")]
        public decimal Propina { get; set; }

        [XmlElement(ElementName = "importeTotal")]
        public required decimal ImporteTotal { get; set; }

        [XmlElement(ElementName = "moneda")]
        public string? Moneda { get; set; } // Opcional

        [XmlElement(ElementName = "pagos")]
        public List<Pago>? Pagos { get; set; } // Opcional

        // [XmlElement(ElementName = "valorRetIva")]
        // public decimal ValorRetIva { get; set; } //Opcional
        // [XmlElement(ElementName = "valorRetRenta")]
        // public decimal ValorRetRenta { get; set; } //Opcional
    }
}