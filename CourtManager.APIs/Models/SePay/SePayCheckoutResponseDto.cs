namespace CourtManager.APIs.Models.SePay;

/// <summary>
/// Data transfer object for SePay checkout response.
/// Returns form action URL and hidden fields for the frontend to auto-submit.
/// SePay checkout works via HTML form POST (browser redirect), not a REST API.
/// </summary>
public class SePayCheckoutResponseDto
{
    /// <summary>
    /// The SePay checkout form action URL.
    /// Frontend should POST a form to this URL with the provided FormFields.
    /// </summary>
    public string CheckoutUrl { get; set; } = string.Empty;

    /// <summary>
    /// Hidden form fields to include in the POST form submission.
    /// Each key-value pair should be rendered as an &lt;input type="hidden"&gt;.
    /// </summary>
    public Dictionary<string, string>? FormFields { get; set; }

    /// <summary>
    /// Success flag.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Optional message.
    /// </summary>
    public string? Message { get; set; }
}
