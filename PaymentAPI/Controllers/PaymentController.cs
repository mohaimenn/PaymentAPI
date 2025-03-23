using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Exceptions;
using PaymentAPI.Models;
using PaymentAPI.Services;

namespace PaymentAPI.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentServiceFactory _paymentServiceFactory;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentServiceFactory paymentServiceFactory, ILogger<PaymentController> logger)
        {
            _paymentServiceFactory = paymentServiceFactory;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Provider))
                    return BadRequest(new { error = "Payment provider is required" });

                if (request.Amount <= 0)
                    return BadRequest(new { error = "Amount must be greater than zero" });

                var paymentService = _paymentServiceFactory.GetPaymentService(request.Provider);
                var response = await paymentService.ProcessPaymentAsync(request);

                _logger.LogInformation("Payment processed for {Provider}: {PaymentId}", response.Provider, response.PaymentId);
                return Ok(response);
            }
            catch (PaymentException ex)
            {
                _logger.LogWarning(ex, "Payment processing failed");
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid payment provider");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during payment processing");
                return StatusCode(500, new { error = "An unexpected error occurred" });
            }
        }
    }
}