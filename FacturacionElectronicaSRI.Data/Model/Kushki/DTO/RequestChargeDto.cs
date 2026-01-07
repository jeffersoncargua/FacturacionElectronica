namespace FacturacionElectronicaSRI.Data.Model.Kushki.DTO
{
    public class RequestChargeDto
    {
        public string token { get; set; }
        public Amount amount { get; set; }
        public Metadata metadata { get; set; }
        public ContactDetails contactDetails { get; set; }
        public OrderDetails orderDetails { get; set; }
        public ProductDetails productDetails { get; set; }
        public string fullResponse { get; set; }
    }

    public class RequestDeferredChargeDto
    {
        public string token { get; set; }
        public Amount amount { get; set; }
        public Metadata metadata { get; set; }
        public ContactDetails contactDetails { get; set; }
        public OrderDetails orderDetails { get; set; }
        public ProductDetails productDetails { get; set; }
        public string fullResponse { get; set; }
        public Deferred deferred { get; set; }
    }

    public class Deferred
    {
        public string graceMonths { get; set; }
        public string creditType { get; set; }
        public int months { get; set; }
    }


    public class Amount
    {
        public double subtotalIva { get; set; }
        public double subtotalIva0 { get; set; }
        public double ice { get; set; }
        public double iva { get; set; }
        public string? currency { get; set; }
    }

    public class BillingDetails
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
    }

    public class ContactDetails
    {
        public string documentType { get; set; }
        public string documentNumber { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
    }

    public class Metadata
    {
        public string Referencia { get; set; }
    }

    public class OrderDetails
    {
        public string siteDomain { get; set; }
        public ShippingDetails shippingDetails { get; set; }
        public BillingDetails billingDetails { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public string title { get; set; }
        public int price { get; set; }
        public string sku { get; set; }
        public int quantity { get; set; }
    }

    public class ProductDetails
    {
        public List<Product> product { get; set; }
    }

    public class ShippingDetails
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
    }
}
