using Microsoft.Extensions.DependencyInjection;

namespace PaymentAPI.Services
{
    public class PaymentServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentService GetPaymentService(string provider)
        {
            return provider.ToLower() switch
            {
                "mollie" => _serviceProvider.GetRequiredService<MolliePaymentService>(),
                "stripe" => _serviceProvider.GetRequiredService<StripePaymentService>(),
                "adyen" => _serviceProvider.GetRequiredService<AdyenPaymentService>(),
                _ => throw new ArgumentException($"Unsupported payment provider: {provider}")
            };
        }
    }
}