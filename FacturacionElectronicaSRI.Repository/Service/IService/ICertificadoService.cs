using org.bouncycastle.util;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace FacturacionElectronicaSRI.Repository.Service.IService
{
    public interface ICertificadoService
    {
        void CargarDesdeAlmacen(Store storename, StoreLocation storeLocation, string friendlyName);
        void CargarDesdeBase64String(string certificadoBase64, string constrasena);
        void CargarDesdeP12(string rutaCertificado, string contrasena);

        XmlDocument FirmarDocumentoXml(string documentPatch);
    }
}