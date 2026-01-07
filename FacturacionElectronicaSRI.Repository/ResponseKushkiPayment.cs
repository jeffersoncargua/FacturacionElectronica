using FacturacionElectronicaSRI.Data.Model.Kushki.DTO;

namespace FacturacionElectronicaSRI.Repository
{
    public class ResponseKushkiPayment
    {
        public Details details { get; set; }

        public string ticketNumber { get; set; }

        public string transactionReference { get; set; }
    }

    public class BinInfo
    {
        public string bank { get; set; }

        public string bindCard { get; set; }

        public string cardCountry { get; set; }

        public string lastFourDigits { get; set; }

        public string type { get; set; }
    }

    public class Details
    {
        public Amount amount { get; set; }
        public string approvalCode { get; set; }
        public string approvedTransactionAmount { get; set; }
        public BinInfo binInfo { get; set; }
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
    }

    public class ExtraTaxes
    {
        public int iac { get; set; }
        public int tasaAeroportuaria { get; set; }
        public int agenciaDeViaje { get; set; }
    }
}