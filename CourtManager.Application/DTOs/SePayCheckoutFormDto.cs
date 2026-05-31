using System.Text.Json.Serialization;

namespace CourtManager.Application.DTOs;

/// <summary>
/// Contains all required fields to build the HTML form for SePay checkout/init.
/// </summary>
public class SePayCheckoutFormDto
{
    [JsonPropertyName("payUrl")]
    public string PayUrl { get; set; } = string.Empty;

    [JsonPropertyName("merchant")]
    public string Merchant { get; set; } = string.Empty;

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = "PURCHASE";

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("order_amount")]
    public string OrderAmount { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "VND";

    [JsonPropertyName("order_invoice_number")]
    public string OrderInvoiceNumber { get; set; } = string.Empty;

    [JsonPropertyName("order_description")]
    public string OrderDescription { get; set; } = string.Empty;

    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("success_url")]
    public string SuccessUrl { get; set; } = string.Empty;

    [JsonPropertyName("error_url")]
    public string ErrorUrl { get; set; } = string.Empty;

    [JsonPropertyName("cancel_url")]
    public string CancelUrl { get; set; } = string.Empty;

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}
