using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentAPI.Exceptions;
using PaymentAPI.Models;
using Stripe;

namespace PaymentAPI.Services;

public class StripePaymentService : IPaymentService
{
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(IOptions<StripeSettings> settings, ILogger<StripePaymentService> logger)
    {
        StripeConfiguration.ApiKey = settings.Value.SecretKey;
        _logger = logger;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = request.Currency.ToLower(),
                Description = request.Description,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            _logger.LogInformation("Stripe PaymentIntent created: {PaymentId}", paymentIntent.Id);

            return new PaymentResponse
            {
                Provider = "Stripe",
                Status = "pending",
                PaymentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Stripe payment");
            throw new PaymentException($"Stripe payment failed: {ex.Message}");
        }
    }
}