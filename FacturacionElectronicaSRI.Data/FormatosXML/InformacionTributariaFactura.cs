using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "infoTributaria")]
    public class InformacionTributariaFactura
    {
        [XmlElement(ElementName = "ambiente")]
        public required string Ambiente { get; set; }

        [XmlElement(ElementName = "tipoEmision")]
        public required string TipoEmision { get; set; }

        [XmlElement(ElementName = "razonSocial")]
        public required string RazonSocial { get; set; }

        [XmlElement(ElementName = "nombreComercial")]
        public required string NombreComercial { get; set; }

        [XmlElement(ElementName = "ruc")]
        public required string Ruc { get; set; }

        [XmlElement(ElementName = "claveAcceso")]
        public required string ClaveAcceso { get; set; }

        [XmlElement(ElementName = "codDoc")]
        public required string CodDoc { get; set; }

        [XmlElement(ElementName = "estab")]
        public required string Estab { get; set; } // Es el codigo del establecimiento de la empresa puede ser 001 hasta 999

        [XmlElement(ElementName = "ptoEmi")]
        public required string PtoEmi { get; set; } // Es el codigo del punto de emision del comprobante desde el establecimiento de la empresa puede ser 001 hasta 999

        [XmlElement(ElementName = "secuencial")]
        public required string Secuencial { get; set; } // Es el numero secuencial del comprobante que va desde 000000001 hasta 999999999

        [XmlElement(ElementName = "dirMatriz")]
        public required string DirMatriz { get; set; }
    }
}