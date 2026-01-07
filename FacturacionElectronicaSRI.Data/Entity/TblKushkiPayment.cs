using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacturacionElectronicaSRI.Data.Entity
{
    public class TblKushkiPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double subtotalIva { get; set; }
        public double subtotalIva0 { get; set; }
        public double ice { get; set; }
        public double iva { get; set; }
        public string? currency { get; set; }
        public string approvalCode { get; set; }
        public string approvedTransactionAmount { get; set; }
        public string bank { get; set; }
        public string? bindCard { get; set; }
        public string cardCountry { get; set; }
        public string lastFourDigits { get; set; }
        public string type { get; set; }
        public string cardHolderName { get; set; }
        public long created { get; set; }
        public string merchantId { get; set; }
        public string merchantName { get; set; }
        public string paymentBrand { get; set; }
        public string processorBankName { get; set; }
        public string requestAmount { get; set; }
        public string responseCode { get; set; }
        public string responseText { get; set; }
        public string transactionId { get; set; }
        public string transactionReference { get; set; }
        public string transactionStatus { get; set; }
        public string transactionType { get; set; }
        public string ticketNumber { get; set; }
    }
}
