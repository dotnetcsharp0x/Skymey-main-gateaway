﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using Skymey_main_gateaway.Data;
using Skymey_main_Gateway;
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
            try
            {
                string url = _optAccess.Value.Server + ":" + _optAccess.Value.Port + "/api/User/GetUsers";
                var client = new RestClient(url);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + token);
                var r = client.ExecuteAsync(request).Result.Content;
                var userd = JsonSerializer.Deserialize<List<SU_001>>(r);
                return Ok(userd);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}