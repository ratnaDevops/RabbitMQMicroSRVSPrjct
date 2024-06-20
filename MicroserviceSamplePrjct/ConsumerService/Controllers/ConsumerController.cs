using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConsumerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private readonly ConsumerService _consumerService;

        public ConsumerController(ConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        await _consumerService.ReceiveMessage();
        //        await Task.Delay(1000, stoppingToken); // Delay for 1 second
        //    }
        //}

        [HttpGet]
        public IActionResult ConsumeMessages()
        {
            string receivedMessage = _consumerService.GetLastReceivedMessage();
            return Ok(receivedMessage);
        }

    }
}

