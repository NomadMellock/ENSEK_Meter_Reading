using ENSEK_Meter_Reading.Models;
using ENSEK_Meter_Reading.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ENSEK_Meter_Reading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly ILogger<MeterReadingController> logger;
        private readonly IFileService uploadService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uploadService">File Service</param>
        /// <param name="logger">default logger</param>
        public MeterReadingController(IFileService uploadService, ILogger<MeterReadingController> logger)
        {
            this.logger = logger;
            this.uploadService = uploadService;
        }


        /// <summary>
        /// Single File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("PostSingleFile")]
        public async Task<ActionResult> PostSingleFile([FromForm] FileUpload fileDetails)
        {
            if (fileDetails == null)
            {
                return BadRequest();
            }
            try
            {
                await uploadService.PostFileAsync(fileDetails.FileDetails, fileDetails.FileType);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Multiple File Upload
        /// </summary>
        /// <returns></returns>
        [HttpPost("PostMultipleFile")]
        public async Task<ActionResult> PostMultipleFile([FromForm] List<FileUpload> fileDetails)
        {
            if (fileDetails == null)
            {
                return BadRequest();
            }
            try
            {
                await uploadService.PostMultiFileAsync(fileDetails);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
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
