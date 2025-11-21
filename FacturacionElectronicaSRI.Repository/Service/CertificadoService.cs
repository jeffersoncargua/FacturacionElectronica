using FacturacionElectronicaSRI.Repository.Service.IService;
using FirmaXadesNetCore;
using FirmaXadesNetCore.Crypto;
using FirmaXadesNetCore.Signature;
using FirmaXadesNetCore.Signature.Parameters;
using Microsoft.Extensions.Logging;
using org.bouncycastle.util;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace FacturacionElectronicaSRI.Repository.Service
{
    public class CertificadoService : ICertificadoService
    {
        internal X509Certificate2 _x509Certificate2;
        private readonly ILogger<CertificadoService> _logger;
        public CertificadoService(ILogger<CertificadoService> logger)
        {
            _logger = logger;
        }

        public void CargarDesdeAlmacen(Store storename, StoreLocation storeLocation, string friendlyName)
        {
            throw new NotImplementedException();
        }

        public void CargarDesdeBase64String(string certificadoBase64, string constrasena)
        {
            throw new NotImplementedException();
        }

        public void CargarDesdeP12(string rutaCertificado, string contrasena)
        {
            _logger.LogTrace("Cargando Certificado desde archivo p12" + rutaCertificado);

            try
            {
                if (File.Exists(rutaCertificado))
                {
                    Console.WriteLine("Si existe");

                    using X509Store store = new("My", StoreLocation.CurrentUser);

                    _x509Certificate2 = new X509Certificate2(File.ReadAllBytes(rutaCertificado), contrasena, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                    _logger.LogDebug("Certificado cargado");
                }

                throw new Exception("No existe el ceritificado P12");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error cargando certificado desde String base64 p12." + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Esta funcion permite firmar un documento para continuar con el proceso de facturacion electronica
        /// Cabe destacar que para que esto se posible, se debe, importar la libreria de FirmaXadesNetCore y sus archivos que estan
        /// en el repositorio de github: https://github.com/newverdun/FirmaXadesNetCore/tree/master/FirmaXadesNetCore.
        /// Tambien se debe generar un proyecto de Biblioteca.Net para poder llamarlo FirmaXadesNetcore.
        /// </summary>
        /// <param name="documentPatch">Es la ruta del documento xml para firmar.</param>
        /// <returns>Retorna un documento XML firmado para continuar con la factuacion electronica.</returns>
        /// <exception cref="Exception">En caso de que no exista un certificado p12 se lanza una exception.</exception>
        public XmlDocument FirmarDocumentoXml(string documentPatch)
        {
            if(_x509Certificate2 == null)
            {
                throw new Exception("Debe primero cargar un certificado válido");
            }

            var xadesService = new XadesService();
            var parameters = new SignatureParameters()
            {
                SignatureMethod = SignatureMethod.RSAwithSHA1,
                DigestMethod = DigestMethod.SHA1,
                SigningDate = new DateTime?(DateTime.Now),
            };

            var signatureCommitment = new SignatureCommitment(SignatureCommitmentType.ProofOfOrigin);
            parameters.SignatureCommitments.Add(signatureCommitment);
            parameters.SignaturePackaging = SignaturePackaging.ENVELOPED;
            SignatureDocument signatureDocument;

            using (parameters.Signer = new Signer(_x509Certificate2))
            {
                using Stream stream = new FileStream(documentPatch, FileMode.Open, FileAccess.Read, FileShare.Read);
                stream.Position = 0L;
                signatureDocument = xadesService.Sign(stream, parameters);
            }

            return signatureDocument.Document;
        }
    }
}