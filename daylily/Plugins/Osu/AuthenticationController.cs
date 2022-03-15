﻿using Coosu.Api.V2;
using Coosu.Api.V2.ResponseModels;
using daylily.Plugins.Osu.Data;
using daylily.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MilkiBotFramework.Plugining.Configuration;

namespace daylily.Plugins.Osu
{
    [Route("api/authentication")]
    [ApiController]
    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006 // 命名样式
    public class AuthenticationController : ControllerBase
#pragma warning restore IDE1006 // 命名样式
    {
        private readonly ILogger _logger;
        private readonly OsuDbContext _osuDbContext;
        private readonly OsuConfig _config;

        public AuthenticationController(ILogger<AuthenticationController> logger,
            IConfiguration<OsuConfig> configuration, 
            OsuDbContext osuDbContext)
        {
            _logger = logger;
            _osuDbContext = osuDbContext;
            _config = configuration.Instance;
        }

        // GET api/<AuthenticationController>
        [HttpGet]
        public async Task<IActionResult> Get(string code, string state)
        {
            string qq;
            try
            {
                var union = EncryptUtil.DecryptAes256UseMd5(state, _config.QQAesKey, _config.QQAesIV);
                var split = union.Split('|');
                qq = split[0];
                var ticks = long.Parse(split[1]);
                var time = new DateTime(ticks);
                if (DateTime.Now - time > TimeSpan.FromMinutes(5))
                {
                    return Content("链接已过期。");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QQ号获取出错，请重试。");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return Content("QQ号获取错误，请重试。");
            }

            UserToken result;
            try
            {
                var authClient = new AuthorizationClient();
                result = await authClient.GetUserToken(_config.ClientId, new Uri(_config.ServerRedirectUri),
                    _config.ClientSecret, code);
                if (result == null)
                    throw new Exception("token result为null");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "token获取出错，请重试。");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return Content("token获取出错，请重试。");
            }

            var client = new OsuClientV2(result);
            var user = await client.User.GetOwnData();
            
            await _osuDbContext.AddOrUpdateToken(long.Parse(qq), user.Id.Value, result);

            return Redirect("https://osu.ppy.sh");
        }
    }
}
