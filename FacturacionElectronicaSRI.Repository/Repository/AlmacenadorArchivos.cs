using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Xml;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class AlmacenadorArchivos : IAlmacenadorArchivos
    {
        private readonly IHostingEnvironment _webHostingEnvironment;
        private readonly IRutasFacturacionRepository _rutasFacturacionRepository;

        public AlmacenadorArchivos(IHostingEnvironment webHostingEnvironment, IRutasFacturacionRepository rutasFacturacionRepository)
        {
            _webHostingEnvironment = webHostingEnvironment;
            _rutasFacturacionRepository = rutasFacturacionRepository;
        }

        public Task BorrarArchivo(string ruta, string contenedor)
        {
            throw new NotImplementedException();
        }

        public Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType, string claveAcceso)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string claveAcceso)
        {
            string ruta = Path.Combine(contenedor, claveAcceso + extension);

            var rutaExist = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == claveAcceso);
            if ((rutaExist.RutaGenerados != null && rutaExist.RutaGenerados == ruta) || (rutaExist.RutaFirmados != null && rutaExist.RutaFirmados == ruta) || (rutaExist.RutaAutorizados != null && rutaExist.RutaAutorizados == ruta))
            {
                var oldFilepath = ruta;
                File.Delete(oldFilepath);
            }

            var resultFile = Encoding.UTF8.GetString(contenido);

            XmlDocument doc = new();
            doc.LoadXml(resultFile); // permite leer un documento xml desde una cadena que contenga un archivo xml directamente
            doc.Save(ruta); // permite almacenar el archivo en la ruta especificada

            return ruta;
        }

        public Task<string> GuardarP12(byte[] contenido, string extension, string contenedor, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarP12Nombre(byte[] contenido, string nombreConExtension, string contenedor, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarP12String(byte[] contenido, string extension, string contenedor, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarXml(byte[] contenido, string extension, string contenedor)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarXmlString(byte[] contenido, string nombreConExtension, string contenedor)
        {
            throw new NotImplementedException();
        }
    }
}