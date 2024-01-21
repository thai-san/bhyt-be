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

                            // Set the start date to now
                            customerPolicy.StartDate = DateTime.UtcNow;

                            // Set the end date based on paymentOption
                            var paymentOption = customerPolicy.PaymentOption;
                            if (paymentOption == false)
                            {
                                customerPolicy.EndDate = DateTime.UtcNow.AddYears(1);
                            }
                            else
                            {
                                customerPolicy.EndDate = DateTime.UtcNow.AddMonths(1);
                            }
                        }

                        // Get the customer's payment
                        var insurancePayment = _context.InsurancePayments.OrderBy(payment=>payment.Id)
                            .LastOrDefault(payment => payment.CustomerId == customerId);

                        if (insurancePayment != null)
                        {
                            // Update the status of the payment
                            insurancePayment.Status = true;

                            // Update the SubscriptionId with the subscription ID from Stripe
                            insurancePayment.SubscriptionId = session.SubscriptionId;

                            var options = new SubscriptionUpdateOptions
                            {
                                Metadata = new Dictionary<string, string>()
                                {
                                    { "customerId", customerId.ToString() }
                                }
                            };
                            var service = new SubscriptionService();
                            Subscription subscription = service.Update(insurancePayment.SubscriptionId, options);
                        }

                        // Save the changes to the database
                        _context.SaveChanges();
                    }
                }
                else if (stripeEvent.Type == Events.InvoicePaymentSucceeded)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    var subscriptionId = invoice.SubscriptionId;

                    // Get the customerId from the subscription metadata
                    var subscriptionService = new SubscriptionService();
                    var subscription = subscriptionService.Get(subscriptionId);
                    var customerIdString = subscription.Metadata["customerId"];

                    if (int.TryParse(customerIdString, out int customerId))
                    {
                        // Create a new insurance payment for the successful payment
                        var newInsurancePayment = new InsurancePayment
                        {
                            Guid = Guid.NewGuid(),
                            CustomerId = customerId,
                            Date = DateTime.UtcNow,
                            Amount = invoice.Total / 100.0, // Stripe amounts are in cents
                            Status = true,
                            Type = "Thanh toán",
                            SubscriptionId = subscriptionId
                        };

                        // Add the new insurance payment to the database
                        _context.InsurancePayments.Add(newInsurancePayment);

                        // Get the customer policy with the customer ID
                        var customerPolicy = _context.CustomerPolicies
                            .FirstOrDefault(policy => policy.CustomerId == customerId);

                        if (customerPolicy != null)
                        {
                            // Update the start date and end date of the policy
                            customerPolicy.StartDate = DateTime.UtcNow;
                            var interval = subscription.Items.Data[0].Price.Recurring.Interval;
                            if (interval == "month")
                            {
                                customerPolicy.EndDate = DateTime.UtcNow.AddMonths(1);
                            }
                            else if (interval == "year")
                            {
                                customerPolicy.EndDate = DateTime.UtcNow.AddYears(1);
                            }
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
