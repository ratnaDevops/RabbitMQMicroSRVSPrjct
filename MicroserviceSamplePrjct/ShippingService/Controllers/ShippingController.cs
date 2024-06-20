using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShippingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingService _shippingService;

        public ShippingController(ShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpGet]
        public IActionResult ProcessShipping()
        {
            //string receivedMessage = _shippingService.GetLastReceivedMessage();
            return Ok("Listening for messages...");
        }

    }
}
