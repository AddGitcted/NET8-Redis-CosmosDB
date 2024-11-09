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
        public class BatchMessageRequest
        {
            public int Count { get; set; }
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


        [HttpPost("batch")]
        public async Task<IActionResult> PostBatch([FromBody] BatchMessageRequest request)
        {
            if (request.Count <= 0)
            {
                return BadRequest("Count must be greater than zero.");
            }

            for (int i = 1; i <= request.Count; i++)
            {
                string message = $"Message generated n°{i}";
                await _producerService.ProduceAsync("topic1", message);
            }

            return Ok($"{request.Count} messages sent successfully.");
        }
    }
}
