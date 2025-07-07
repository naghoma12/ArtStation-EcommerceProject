using Org.BouncyCastle.Asn1.X9;

namespace ArtStation.Dtos.PaymobDtos
{
    public class PaymobCallbackDto
    {
        public string Type { get; set; }
        public TransactionObj Obj { get; set; }
    }

    public class TransactionObj
    {
        public int Id { get; set; }
        public bool Pending { get; set; }
        public int AmountCents { get; set; }
        public bool Success { get; set; }
        public bool IsAuth { get; set; }
        public bool IsCapture { get; set; }
        public bool IsStandalonePayment { get; set; }
        public bool IsVoided { get; set; }
        public bool IsRefunded { get; set; }
        public bool Is3dSecure { get; set; }
        public int IntegrationId { get; set; }
        public int ProfileId { get; set; }
        public bool HasParentTransaction { get; set; }
        public OrderInfo Order { get; set; }
        public string CreatedAt { get; set; }
        public string Currency { get; set; }
        public SourceData SourceData { get; set; }
        public string ApiSource { get; set; }
        public PaymentKeyClaims PaymentKeyClaims { get; set; }
        public TransactionData Data { get; set; }
    }
    public class OrderInfo
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public bool DeliveryNeeded { get; set; }
        public Merchant Merchant { get; set; }
        public int AmountCents { get; set; }
        public string Currency { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsReturned { get; set; }
        public bool IsPaymentLocked { get; set; }
        public int PaidAmountCents { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class Merchant
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public List<string> Phones { get; set; }
        public List<string> CompanyEmails { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
    public class SourceData
    {
        public string Pan { get; set; }
        public string Type { get; set; }       // e.g., card, wallet
        public string SubType { get; set; }    // e.g., MasterCard
    }
    public class PaymentKeyClaims
    {
        public int UserId { get; set; }
        public string Currency { get; set; }
        public int OrderId { get; set; }
        public int AmountCents { get; set; }
        public BillingData BillingData { get; set; }
        public string RedirectUrl { get; set; }
        public int IntegrationId { get; set; }
    }
    public class BillingData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class TransactionData
    {
        public string MigsResult { get; set; }
        public string Message { get; set; }
        public double CapturedAmount { get; set; }
        public double AuthorisedAmount { get; set; }
        public string CardType { get; set; }
        public string CardNum { get; set; }
        public string TxnResponseCode { get; set; }
        public string MerchantTxnRef { get; set; }
    }



}
