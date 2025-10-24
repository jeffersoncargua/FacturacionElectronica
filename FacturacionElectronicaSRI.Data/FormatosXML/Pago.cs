using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "pago")]
    public class Pago
    {
        [XmlElement(ElementName = "formaPago")]
        public required string FormaPago { get; set; }

        [XmlElement(ElementName = "total")]
        public required decimal Total { get; set; }

        [XmlElement(ElementName = "plazo")]
        public required string Plazo { get; set; }

        [XmlElement(ElementName = "unidadTiempo")]
        public required string UnidadTiempo { get; set; }
    }
}