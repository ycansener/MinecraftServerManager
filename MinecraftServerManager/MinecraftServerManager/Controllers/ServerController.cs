using Common;
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace MinecraftServerManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly IOptions<ServerConfig> _serverConfig;
        private readonly RedisPubSubService _redisPubSubService;

        public ServerController(IOptions<ServerConfig> serverConfig)
        {
            _serverConfig = serverConfig;
            _redisPubSubService = new RedisPubSubService(serverConfig.Value.PubSubConnectionString);
        }

        [HttpGet]
        public ActionResult<string> GetServerStatus([FromQuery] string secret)
        {
            if(!IsServerRunnning(_serverConfig.Value))
            {
                return NotFound("Minecraft server is not running at this moment!");
            }

            return Ok("Server is up and running.");
        }

        [HttpGet("start")]
        public async Task<ActionResult<string>> RunServer([FromQuery] string secret)
        {
            try
            {
                await _redisPubSubService.PublishAsync(Constants.CHANNELNAME, Constants.START);
                return Ok("Server has been started.");
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong: {ex.ToString()}");
            }
        }


        [HttpGet("stop")]
        public async Task<ActionResult<string>> StopServer([FromQuery] string secret)
        {
            try
            {
                await _redisPubSubService.PublishAsync(Constants.CHANNELNAME, Constants.STOP);
                return Ok("Server has been stopped.");
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong: {ex.Message}");
            }
        }

        private bool IsServerRunnning(ServerConfig serverConfig)
        {
            Process[] serverProcesses = Process.GetProcessesByName(serverConfig.ProcessName);

            if (serverProcesses.Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}
