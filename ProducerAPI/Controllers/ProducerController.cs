using Microsoft.AspNetCore.Mvc;
using ProducerAPI.Services;

namespace ProducerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly IKafkaProducerService _producerService;

        public ProducerController(IKafkaProducerService producerService)
        {
            _producerService = producerService;
        }

        public class MessageRequest
        {
            public string Message { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            await _producerService.ProduceAsync("topic1", request.Message);
            return Ok("Message sent successfully.");
        }

    }
}
