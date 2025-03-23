using Adyen.Model.Checkout;
using Adyen.Service.Checkout;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentAPI.Exceptions;
using PaymentAPI.Models;

namespace PaymentAPI.Services
{
    public class AdyenPaymentService : IPaymentService
    {
        private readonly PaymentsService _paymentsService;
        private readonly ILogger<AdyenPaymentService> _logger;
        private readonly AdyenSettings _settings;

        public AdyenPaymentService(IOptions<AdyenSettings> settings, ILogger<AdyenPaymentService> logger)
        {
            _settings = settings.Value;
            var config = new Adyen.Config
            {
                XApiKey = _settings.ApiKey,
                Environment = Adyen.Model.Environment.Test
            };
            var client = new Adyen.Client(config);
            _paymentsService = new Adyen.Service.Checkout.PaymentsService(client);
            _logger = logger;
        }

        public async Task<PaymentAPI.Models.PaymentResponse> ProcessPaymentAsync(PaymentAPI.Models.PaymentRequest request)
        {
            try
            {
                var adyenRequest = new Adyen.Model.Checkout.PaymentRequest
                {
                    Amount = new Amount(request.Currency, (long)(request.Amount * 100)),
                    Reference = Guid.NewGuid().ToString(),
                    ReturnUrl = request.RedirectUrl,
                    MerchantAccount = _settings.MerchantAccount,
                    PaymentMethod = new CheckoutPaymentMethod(new CardDetails
                    {
                        Type = CardDetails.TypeEnum.Scheme
                    })
                };

                var adyenResponse = await _paymentsService.PaymentsAsync(adyenRequest);
                _logger.LogInformation("Adyen payment initiated: {PspReference}", adyenResponse.PspReference);

                return new PaymentAPI.Models.PaymentResponse
                {
                    Provider = "Adyen",
                    Status = "pending",
                    PaymentId = adyenResponse.PspReference,
                    CheckoutUrl = adyenResponse.PspReference
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Adyen payment");
                throw new PaymentException($"Adyen payment failed: {ex.Message}");
            }
        }
    }
}