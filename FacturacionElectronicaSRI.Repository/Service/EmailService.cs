using FacturacionElectronica.Utility;
using FacturacionElectronicaSRI.Repository.Service.IService;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace FacturacionElectronicaSRI.Repository.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            // string file1 = "C:\\Users\\Juan Medina\\source\\repos\\Facturacion Electronica API\\Facturacion Electronica\\Archivos\\FacturasPDF\\Factura-1111202501170799690400110010010000000010451508211.pdf";
            // string file2 = "C:\\Users\\Juan Medina\\source\\repos\\Facturacion Electronica API\\Facturacion Electronica\\Archivos\\FacturasPDF\\Factura-1111202501170799690400110010010000000010667190711.pdf";
            emailMessage.From.Add(new MailboxAddress("Factura Electronica", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            // emailMessage.Body = new TextPart(TextFormat.Text) { Text = message.Content };

            // Se realiza la configuracion para enviar archivos en el mensaje de correo
            // En este segmento se configura el primer archivo que seria la factura en pdf
            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                var attachment1 = new MimePart()
                {
                    Content = new MimeContent(File.OpenRead(message.Attachments[1]), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(message.Attachments[1]),
                };

                // En este segmento se configura el segundo archivo que seria la factura en xml
                var attachment2 = new MimePart()
                {
                    Content = new MimeContent(File.OpenRead(message.Attachments[0]), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(message.Attachments[0]),
                };

                // Se configura el multipart para que se pueda enviar los archivos y el mensaje si es necesario
                var multipart = new Multipart("mixed");
                multipart.Add(new TextPart(TextFormat.Text) { Text = message.Content });
                multipart.Add(attachment1);
                multipart.Add(attachment2);

                // Para enviar los archivos en el cuerpo del mensaje se emplea el body y se envia el multipart para que se coloquen en el mensaje que recibira el cliente
                emailMessage.Body = multipart;
            }
            else
            {
                emailMessage.Body = new TextPart(TextFormat.Text) { Text = message.Content };
            }

            return emailMessage;
        }
    }
}