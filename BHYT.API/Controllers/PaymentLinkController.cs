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

        private readonly BHYTDbContext _context;

        public PaymentLinkController(BHYTDbContext context, IMapper mapper)
        {
            _context = context;
        }

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
                        Recurring = new SessionLineItemPriceDataRecurringOptions
                        {
                            Interval = paymentLinkDTO.PaymentOption, // "month" or "year"
                        },
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = paymentLinkDTO.ProductName,
                        },
                    },
                    Quantity = 1,
                },
            },
                    Mode = "subscription",
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

        [HttpPost("cancel")]
        public IActionResult CancelSubscription([FromBody] string subscriptionId)
        {
            try
            {
                StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

                var options = new SubscriptionUpdateOptions { CancelAtPeriodEnd = true };
                var service = new SubscriptionService();

                service.Update(subscriptionId, options);

                // Get the insurance payment with the subscription ID
                var insurancePayment = _context.InsurancePayments
                    .FirstOrDefault(payment => payment.SubscriptionId == subscriptionId);

                if (insurancePayment != null)
                {
                    // Set the SubscriptionId to null
                    insurancePayment.SubscriptionId = null;

                    // Save the changes to the database
                    _context.SaveChanges();
                }

                return Ok(new { status = "Subscription will be cancelled at the end of the current period" });
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}
