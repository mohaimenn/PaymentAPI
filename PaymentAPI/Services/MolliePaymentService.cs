using Microsoft.Extensions.Options;
using Mollie.Api.Client;
using Mollie.Api.Models;
using MolliePaymentRequest = Mollie.Api.Models.Payment.Request.PaymentRequest;
using PaymentAPI.Models;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using PaymentAPI.Exceptions;

namespace PaymentAPI.Services;

public class MolliePaymentService : IPaymentService
{
    private readonly PaymentClient _paymentClient;
    private readonly ILogger<MolliePaymentService> _logger;

    public MolliePaymentService(IOptions<MollieSettings> settings, ILogger<MolliePaymentService> logger)
    {
        _paymentClient = new PaymentClient(settings.Value.ApiKey);
        _logger = logger;
    }
    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var mollieRequest = new Mollie.Api.Models.Payment.Request.PaymentRequest
            {
                Amount = new Amount(request.Currency, request.Amount),
                Description = request.Description,
                RedirectUrl = request.RedirectUrl
            };

            var mollieResponse = await _paymentClient.CreatePaymentAsync(mollieRequest);
            _logger.LogInformation("Mollie payment created: {PaymentId}", mollieResponse.Id);

            return new PaymentResponse
            {
                Provider = "Mollie",
                Status = mollieResponse.Status,
                PaymentId = mollieResponse.Id,
                CheckoutUrl = mollieResponse.Links.Checkout?.Href ?? throw new PaymentException("Mollie checkout URL unavailable")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Mollie payment");
            throw new PaymentException($"Mollie payment failed: {ex.Message}");
        }
    }
}
