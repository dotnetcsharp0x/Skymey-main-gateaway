using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nancy.Json;
using RestSharp;
using Skymey_main_gateaway.Data;
using Skymey_main_Gateway;
using Skymey_main_Gateway.Models.JWT;
using Skymey_main_Gateway.Models.Tables.User;
using System.Text.Json;

namespace Skymey_main_gateaway.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IOptions<UserSettings> _optAccess;
        private readonly IOptions<JWTSettings> _optAccessJWT;
        public UserController(ILogger<UserController> logger, IOptions<UserSettings> optAccess, IOptions<JWTSettings> optAccessJWT)
        {
            _logger = logger;
            _optAccess = optAccess;
            _optAccessJWT = optAccessJWT;
        }
        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult> GetUsers(string token)
        {
            string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/User/GetUsers", Method.Get);
                request.AddHeader("Authorization", "Bearer " + token);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<List<SU_001>>(r);
                return Ok(userd);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message + url);
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenApiModel tokenApiModel)
        {
            string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/User/refresh", Method.Post);
                request.AddHeader("Authorization", "Bearer " + tokenApiModel.AccessToken);
                var json = new JavaScriptSerializer().Serialize(tokenApiModel);
                var body = json;
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<AuthenticatedResponse>(r);
                return Ok(userd);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + url);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(SU_001 user,string token)
        {
            string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/User/Login", Method.Post);
                request.AddHeader("Authorization", "Bearer " + token);
                var json = new JavaScriptSerializer().Serialize(user);
                var body = json;
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<AuthenticatedResponse>(r);
                return Ok(userd);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + url);
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(SU_001 user,string token)
        {
            string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/User/Register", Method.Post);
                request.AddHeader("Authorization", "Bearer " + token);
                var json = new JavaScriptSerializer().Serialize(user);
                var body = json;
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<AuthenticatedResponse>(r);
                return Ok(userd);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + url);
            }
        }
    }
}
