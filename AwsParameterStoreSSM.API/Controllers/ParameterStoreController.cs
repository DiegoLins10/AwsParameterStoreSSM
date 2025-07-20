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
        private readonly IConfiguration _configuration;
        private readonly AwsParameterCommon _settings;
        private readonly AwsParameterCommon _settingsOptionsMonitor;
        private readonly AwsParameterCommon _settingsOptionsSnapshot;

        public ParameterStoreController(IConfiguration configuration,
            IOptions<AwsParameterCommon> settings,
            IOptionsMonitor<AwsParameterCommon> optionsMonitor,
            IOptionsSnapshot<AwsParameterCommon> optionsSnapshot
            )
        {
            _configuration = configuration;
            _settings = settings.Value;
            _settingsOptionsMonitor = optionsMonitor.CurrentValue;
            _settingsOptionsSnapshot = optionsSnapshot.Value;
        }

        // GET: api/v1/parameterstore/connection-string
        // 📌 Comparison of different AWS SSM configuration binding methods:
        //
        // - fromOption:
        //   Reads value via IOptions<T>. This value is loaded once at app startup.
        //   ➤ Does NOT reflect changes in SSM unless the app is restarted.
        //
        // - fromConfig:
        //   Reads value directly from IConfiguration (bound to SSM via provider).
        //   ➤ Reflects real-time changes from Parameter Store without app restart.
        //
        // - fromOptionMonitor:
        //   Uses IOptionsMonitor<T>. Tracks real-time updates in configuration.
        //   ➤ Reflects changes immediately during app execution. (DANGEROUS)
        //
        // - fromOptionSnapshot:
        //   Uses IOptionsSnapshot<T>. Captures config value **per request scope**.
        //   ➤ Useful in scoped services (e.g. per HTTP request).
        //   ➤ Updates are reflected on new requests only (current request stays consistent).
        //
        // Requires: Amazon.Extensions.Configuration.SystemsManager NuGet package.
        [HttpGet("connection-string")]
        public IActionResult Get()
        {
            var value = _configuration.GetValue<string>("Settings:ConnectionString");
            return Ok(new
            {
                fromOption = _settings.ConnectionString,
                fromConfig = value,
                fromOptionMonitor = _settingsOptionsMonitor.ConnectionString,
                fromOptionSnapshot = _settingsOptionsSnapshot.ConnectionString,
            });
        }
    }
}