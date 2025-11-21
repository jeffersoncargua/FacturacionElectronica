namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IAlmacenadorArchivos
    {
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType, string claveAcceso);
        Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string claveAcceso);
        Task BorrarArchivo(string ruta, string contenedor);
        Task<string> GuardarP12(byte[] contenido, string extension, string contenedor, string contentType);
        Task<string> GuardarXml(byte[] contenido, string extension, string contenedor);
        Task<string> GuardarP12String(byte[] contenido, string extension, string contenedor, string contentType);
        Task<string> GuardarXmlString(byte[] contenido, string nombreConExtension, string contenedor);
        Task<string> GuardarP12Nombre(byte[] contenido, string nombreConExtension, string contenedor, string contentType);
    }
}