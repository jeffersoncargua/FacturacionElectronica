using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "detAdicional")]
    public class DetalleAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string? Nombre { get; set; }

        [XmlAttribute(AttributeName = "valor")]
        public string? Valor { get; set; }
    }
}