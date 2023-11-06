using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nancy.Json;
using RestSharp;
using Skymey_main_gateaway.Data;
using Skymey_main_Gateway;
using Skymey_main_lib.Models.JWT;
using Skymey_main_lib.Models.Tables.User;
using System.Net;
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
        public UserController(
            ILogger<UserController> logger, 
            IOptions<UserSettings> optAccess, 
            IOptions<JWTSettings> optAccessJWT
        )
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
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<List<SU_001ListViewModel>>(r);
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
        public async Task<IActionResult> Login(SU_001LoginViewModel user)
        {
            SU_001 user_to_send = new SU_001();
            user_to_send.Email = user.Email;
            user_to_send.Password = user.Password;
            user_to_send.FirstName = "";
            user_to_send.LastName = "";
            user_to_send.RefreshToken = "";
            user_to_send.RefreshTokenExpiryTime = DateTime.UtcNow;
            string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port;
            HttpStatusCode status_code=HttpStatusCode.Accepted;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/User/Login", Method.Post);
                var json = new JavaScriptSerializer().Serialize(user_to_send);
                var body = json;
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var resp = client.ExecuteAsync(request).Result;
                status_code = resp.StatusCode;
                if (status_code == HttpStatusCode.OK)
                {
                    return Ok(JsonSerializer.Deserialize<AuthenticatedResponse>(resp.Content));
                }
                else
                {
                    return StatusCode(Convert.ToInt32(status_code));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Convert.ToInt32(status_code));
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(SU_001RegisterViewModel user)
        {
            SU_001 user_to_send = new SU_001();
            user_to_send.Email = user.Email;
            user_to_send.Password = user.Password;
            user_to_send.FirstName = user.FirstName;
            user_to_send.LastName = user.LastName;
            user_to_send.RefreshToken = "";
            user_to_send.RefreshTokenExpiryTime = DateTime.UtcNow;
            string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port;
            HttpStatusCode status_code = HttpStatusCode.Accepted;
            try
            {
                var options = new RestClientOptions(url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/User/Register", Method.Post);
                var json = new JavaScriptSerializer().Serialize(user);
                var body = json;
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var resp = client.ExecuteAsync(request).Result;
                status_code = resp.StatusCode;
                if (status_code == HttpStatusCode.OK)
                {
                    return Ok(JsonSerializer.Deserialize<AuthenticatedResponse>(resp.Content));
                }
                else
                {
                    return StatusCode(Convert.ToInt32(status_code));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + url);
            }
        }
    }
}
