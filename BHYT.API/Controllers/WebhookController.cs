using AutoMapper;
using BHYT.API.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BHYT.API.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly BHYTDbContext _context;

        public WebhookController(BHYTDbContext context, IMapper mapper)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    var customerIdString = session.Metadata["customerId"];

                    if (int.TryParse(customerIdString, out int customerId))
                    {
                        var customerPolicy = _context.CustomerPolicies
                        .FirstOrDefault(policy => policy.CustomerId == customerId);

                        if (customerPolicy != null)
                        {
                            // Update the status of the policy
                            customerPolicy.Status = true;
                        }

                        // Get the customer's payment
                        var insurancePayment = _context.InsurancePayments
                            .FirstOrDefault(payment => payment.CustomerId == customerId);

                        if (insurancePayment != null)
                        {
                            // Update the status of the payment
                            insurancePayment.Status = true;
                        }

                        // Save the changes to the database
                        _context.SaveChanges();
                    }
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
