using AutoMapper;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Session = Stripe.Checkout.Session;

namespace BHYT.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentLinkController : ControllerBase
    {
        [HttpPost("{customerId}")]
        public IActionResult CreatePaymentLink(int customerId, [FromBody] PaymentLinkDTO paymentLinkDTO)
        {
            try
            {
                StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long?)paymentLinkDTO.Amount,
                                Currency = "vnd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = paymentLinkDTO.ProductName,
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = "https://example.com",
                    CancelUrl = "https://example.com",
                    Metadata = new Dictionary<string, string>()
                    {
                        { "customerId", customerId.ToString() }
                    }
                };

                var service = new SessionService();
                Session session = service.Create(options);

                return Ok(new { link = session.Url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
