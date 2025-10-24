using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "impuesto")]
    public class Impuesto
    {
        [XmlElement(ElementName = "codigo")]
        public required string Codigo { get; set; }

        [XmlElement(ElementName = "codigoPorcentaje")]
        public required string CodigoPorcentaje { get; set; }

        [XmlElement(ElementName = "tarifa")]
        public required decimal Tarifa { get; set; }

        [XmlElement(ElementName = "baseImponible")]
        public required decimal BaseImponible { get; set; }

        [XmlElement(ElementName = "valor")]
        public required decimal Valor { get; set; }
    }
}