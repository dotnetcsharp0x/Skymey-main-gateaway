using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using Skymey_main_gateaway.Data;
using Skymey_main_lib.Models.Prices.CurrentPricesViewModel;
using Skymey_main_lib.Models.Prices.StockPrices;
using Skymey_main_lib.Models.Prices.StockPricesMongo;
using System.Text.Json;

namespace Skymey_main_gateaway.Controllers
{
    [Route("api/Stock")]
    [ApiController]
    public class StockController : Controller
    {

        private readonly ILogger<StockController> _logger;
        private readonly IOptions<MongoDbSettings> _optMongo;
        public StockController(
            ILogger<StockController> logger,
            IOptions<MongoDbSettings> optMongo
            )
        {
            _logger = logger;
            _optMongo = optMongo;
        }
        [HttpGet]
        [Route("GetPrices")]
        public IActionResult GetPrices()
        {
            string url = _optMongo.Value.Server + ":" + _optMongo.Value.Port;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/Stock/GetPrices", Method.Get);
                //request.AddHeader("Authorization", "Bearer " + token);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<List<StockPricesMongo>>(r);
                var ExchangesVM = (from i in userd select new CurrentPricesViewModel { Ticker = i.Ticker, Figi = i.Figi, Price = i.Price, Currency = i.Currency, Update = i.Update });
                return Ok(ExchangesVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + url);
            }
        }
    }
}

