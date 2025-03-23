namespace PaymentAPI.Models;

public class AdyenSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string MerchantAccount { get; set; } = string.Empty;
    public string HmacSecret { get; set; } = string.Empty;
}
