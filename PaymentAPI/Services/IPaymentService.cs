using PaymentAPI.Models;

namespace PaymentAPI.Services;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
}
