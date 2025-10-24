using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Data.FormatosXML
{
    [XmlRoot(ElementName = "detalle")]
    public class Detalle
    {
        [XmlElement(ElementName = "codigoPrincipal")]
        public required string CodigoPrincipal { get; set; }

        [XmlElement(ElementName = "codigoAuxiliar")]
        public string? CodigoAuxiliar { get; set; } // Opcional

        [XmlElement(ElementName = "descripcion")]
        public required string Descripcion { get; set; }

        [XmlElement(ElementName = "cantidad")]
        public required decimal Cantidad { get; set; }

        [XmlElement(ElementName = "precioUnitario")]
        public required decimal PrecioUnitario { get; set; }

        [XmlElement(ElementName = "descuento")]
        public required decimal Descuento { get; set; }

        [XmlElement(ElementName = "precioTotalSinImpuesto")]
        public required decimal PrecioTotalSinImpuesto { get; set; }

        [XmlElement(ElementName = "detallesAdicionales")]
        public List<DetalleAdicional>? DetallesAdicionales { get; set; } // Opcional

        [XmlElement(ElementName = "impuestos")]
        public required List<Impuesto> Impuestos { get; set; }
    }
}