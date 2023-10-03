using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ENSEK_Meter_Reading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly ILogger<MeterReadingController> logger;

        public MeterReadingController(ILogger<MeterReadingController> logger)
        {
            this.logger = logger;
        }

        // GET: api/<MeterReadingController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MeterReadingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MeterReadingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MeterReadingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MeterReadingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
