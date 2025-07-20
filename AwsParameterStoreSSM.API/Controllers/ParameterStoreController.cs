using AwsParameterStoreSSM.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AwsParameterStoreSSM.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ParameterStoreController : ControllerBase
    {
        private readonly  IConfiguration _configuration;
        private readonly AwsParameterCommon _settings;

        public ParameterStoreController(IConfiguration configuration, IOptions<AwsParameterCommon> settings)
        {
            _configuration = configuration;
            _settings = settings.Value;
        }


        // GET: api/<ParameterStoreController>
        // fromOption: Take from SSM using IOptions
        // fromConfig: Take from IConfiguration directly into aws SSM
        // Amazon.Extensions.Configuration.SystemsManager
        [HttpGet("connection-string")]
        public IActionResult Get()
        {
            var value = _configuration.GetValue<string>("Settings:ConnectionString");
            return Ok(new
            {
                fromOption = _settings.ConnectionString, 
                fromConfig = value          
            });
        }
    }
}
