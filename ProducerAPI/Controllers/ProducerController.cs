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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            await _producerService.ProduceAsync("topic1", message);
            return Ok("Message sent successfuly.");
        }
    }
}
