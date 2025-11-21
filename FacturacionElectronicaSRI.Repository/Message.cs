using MimeKit;

namespace FacturacionElectronicaSRI.Repository
{
    public class Message
    {
        public Message(IEnumerable<string> to, string subject, string content, IEnumerable<string> attachments)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
            Attachments = new List<string>();
            Attachments.AddRange(attachments);
        }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
            Attachments = new List<string>();
        }

        public List<MailboxAddress> To { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public List<string>? Attachments { get; set; }
        }
}