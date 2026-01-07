namespace FacturacionElectronicaSRI.Data.Model.Kushki.DTO
{
    public class RequestTokenDto
    {
        public Card card { get; set; }
        public double totalAmount { get; set; }
        public string currency { get; set; }

        public bool isDeferred { get; set; } = false;
    }

    public class Card
    {
        public string name { get; set; }
        public string number { get; set; }
        public string expiryMonth { get; set; }
        public string expiryYear { get; set; }
        public string cvv { get; set; }
    }

}
