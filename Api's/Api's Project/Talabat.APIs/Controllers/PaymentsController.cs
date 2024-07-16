using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentServices _paymentServices;
        const string endpointSecret = "whsec_cefaff31899633f0a78594824205266f9274591279190ae2cf9ca951eb38d066";

        public PaymentsController(IPaymentServices paymentServices) 
        {
            _paymentServices = paymentServices;
        }


        //

        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost] // POST : /api/payments
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentId(string basketId)
        {
         var basket = await  _paymentServices.CreateOrUpdatePaymentEntity(basketId);
            if(basket == null)
            {
                return BadRequest(new ApiResponse(statusCode:400, "There is a Problem With Your Basket!"));
            }

            return Ok(basket);
        }


        [AllowAnonymous]
        [HttpPost(template: "webhook")] // POST: https://localhost:7228/api/payments/webhook
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

              var paymentIntent =  stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {

                  await  _paymentServices.UpdatePaymentIntintToSuccessedOrFailed(paymentIntent.Id, flag: false);

                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                  await  _paymentServices.UpdatePaymentIntintToSuccessedOrFailed(paymentIntent.Id, flag: true);
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
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
