using System.Text;

namespace CourtManager.APIs.Configuration;

public class SePaySettings
{
    public string MerchantId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string HmacSecret { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string Environment { get; set; } = "sandbox";
    public string WebhookApiKey { get; set; } = string.Empty;
    public string ExpectedTransferType { get; set; } = "in";
    public string SuccessUrl { get; set; } = string.Empty;
    public string ErrorUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;

    // Bank account details for QR and Checkout Page
    public string BankId { get; set; } = "tpb"; 
    public string AccountNo { get; set; } = "84519828888";
    public string AccountName { get; set; } = "PHAM DUC HUNG"; 
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;

    public string EffectiveMerchantId => string.IsNullOrWhiteSpace(MerchantId) ? ApiKey : MerchantId;
    public string EffectiveSecretKey => string.IsNullOrWhiteSpace(SecretKey) ? ApiSecret : SecretKey;
    public string EffectiveWebhookApiKey => string.IsNullOrWhiteSpace(WebhookApiKey) ? ApiKey : WebhookApiKey;

    public string BasicAuthCredentials => 
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{EffectiveMerchantId}:{EffectiveSecretKey}"));

    // SePay Payment Gateway URLs (Sandbox)
    public string ApiBaseUrl => "https://pgapi-sandbox.sepay.vn";
    public string CheckoutBaseUrl => "https://pay-sandbox.sepay.vn";
}
