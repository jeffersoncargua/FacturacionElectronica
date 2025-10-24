using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "campoAdicional")]
    public class CampoAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string? Nombre { get; set; }
    }
}