using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using Skymey_main_gateaway.Data;
using Skymey_main_lib.Models.CryptoCurrentPricesView;
using Skymey_main_lib.Models.Prices.CurrentPricesViewModel;
using Skymey_main_lib.Models.Prices.StockPrices;
using Skymey_main_lib.Models.Prices.StockPricesMongo;
using Skymey_main_lib.Models.Tickers;
using Skymey_main_lib.Models.Tickers.Polygon;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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
        [HttpGet]
        [Route("GetShares")]
        public async Task<IActionResult> GetShares()
        {
            try
            {
                var userd = await new HttpClient().GetFromJsonAsync<TickerList[]>(_optMongo.Value.Server + ":" + _optMongo.Value.Port + "/api/Stock/GetShares");
                var ExchangesVM = (from i in userd select new SharesList { Ticker = i.ticker, Name = i.name, Market = i.market, Locale = i.locale, Type = i.type, Currency_name = i.currency_name,
                Last_updated_utc = i.last_updated_utc, Composite_figi = i.composite_figi, Share_class_figi = i.share_class_figi, Primary_exchange = i.primary_exchange, Cik = i.cik, Update = i.Update});
                return Ok(ExchangesVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

