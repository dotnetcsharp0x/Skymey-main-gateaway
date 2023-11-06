using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using Skymey_main_gateaway.Data;
using Skymey_main_lib.Models;
using System.Text.Json;

namespace Skymey_main_gateaway.Controllers
{
    [Route("api/Crypto")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly ILogger<CryptoController> _logger;
        private readonly IOptions<MongoDbSettings> _optMongo;
        public CryptoController(
            ILogger<CryptoController> logger,
            IOptions<MongoDbSettings> optMongo
            ) {
            _logger = logger;
            _optMongo = optMongo;
        }
        [HttpGet]
        [Route("GetExchanges")]
        public IActionResult Get()
        {
            string url = _optMongo.Value.Server + ":" + _optMongo.Value.Port;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/Mongo/GetExchanges", Method.Get);
                //request.AddHeader("Authorization", "Bearer " + token);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<HashSet<Exchanges>>(r);
                var ExchangesVM = (from i in userd select new ExchangesViewModel { Name = i.Name,Blockchain=i.Blockchain,Pairs=i.Pairs,Trades=i.Trades,Type=i.Type,Volume24h=i.Volume24h});
                return Ok(ExchangesVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + url);
            }
        }
    }
}
