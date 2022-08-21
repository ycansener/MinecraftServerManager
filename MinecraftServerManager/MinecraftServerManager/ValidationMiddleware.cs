using Common.Models;
using Microsoft.Extensions.Options;

namespace MinecraftServerManager
{
    public class ValidationMiddleware : IMiddleware
    {
        private readonly IOptions<ServerConfig> serverConfig;

        public ValidationMiddleware(IOptions<ServerConfig> serverConfig)
        {
            this.serverConfig = serverConfig;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var secret = context.Request.Query["secret"];

            
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("/server") && !ValidateSecret(serverConfig.Value, secret))
            {
                throw new Exception("Invalid secret!");
            }

            return next.Invoke(context);
        }

        private bool ValidateSecret(ServerConfig serverConfig, string secret)
        {
            if (serverConfig.Secret == secret)
            {
                return true;
            }

            return false;
        }
    }
}
