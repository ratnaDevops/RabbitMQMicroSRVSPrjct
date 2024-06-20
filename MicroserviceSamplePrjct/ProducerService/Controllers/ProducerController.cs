using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProducerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly ProducerService _producerService;

        public ProducerController(ProducerService producerService)
        {
            _producerService = producerService;
        }

        [HttpPost]
        public IActionResult SendMessage([FromBody] string message)
        {
            _producerService.SendMessage(message);
            return Ok();
        }
    }
}
