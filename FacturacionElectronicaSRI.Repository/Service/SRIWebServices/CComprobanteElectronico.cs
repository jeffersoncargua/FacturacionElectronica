namespace FacturacionElectronicaSRI.Repository.Service.SRIWebServices
{
    public class CComprobanteElectronico
    {
        public CRespuestaRecepcion RecepcionComprobantePrueba(string path)
        {
            var mensaje = string.Empty;
            var xmlByte = File.ReadAllBytes(path);
            CSRIws sri = new ();

            CRespuestaRecepcion resRecepcion = sri.RecepcionComprobanteOnLinePrueba(Convert.ToBase64String(xmlByte));

            if (resRecepcion.Estado.Equals("DEVUELTA"))
            {
                foreach (var comprobanteRecepcion in resRecepcion.Comprobantes)
                {
                    foreach (var mensajeComprobante in comprobanteRecepcion.Mensajes)
                    {
                        mensaje = mensajeComprobante.mensaje;
                        mensaje = mensajeComprobante.InformacionAdicional;
                        mensaje = mensajeComprobante.Identificador;
                        mensaje = mensajeComprobante.Tipo;
                    }
                }
            }

            return resRecepcion;
        }

        public CRespuestaAutorizacion AutorizacionComprobantePrueba(string claveAcceso)
        {
            CSRIws sri = new ();
            CRespuestaAutorizacion resAutorizacion = sri.AutorizacionComprobanteOnLinePrueba(claveAcceso);
            return resAutorizacion;
        }
    }
}