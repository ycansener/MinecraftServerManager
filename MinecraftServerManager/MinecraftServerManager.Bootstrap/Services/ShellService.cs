using Common;
using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Bootstrap.Services
{
    public class ShellService
    {
        private readonly ServerConfig _config;

        public ShellService(ServerConfig config)
        {
            _config = config;
        }

        public async Task Start()
        {
            RedisPubSubService redisPubSubService = new RedisPubSubService(_config.PubSubConnectionString);

            await redisPubSubService.SubscribeAsync(Constants.CHANNELNAME, (string channel, string message) =>
            {
                ProcessMessage(message);
            });

            await redisPubSubService.WaitUntilTerminationAsync();
        }

        public async Task Stop()
        {
            RedisPubSubService redisPubSubService = new RedisPubSubService(_config.PubSubConnectionString);

            await redisPubSubService.PublishAsync(Constants.CHANNELNAME, Constants.TERMINATE);
        }

        private void ProcessMessage(string message)
        {
            switch (message)
            {
                case Constants.START:
                    StartServer(_config);
                    break;
                case Constants.STOP:
                    StopServer(_config);
                    break;
                case Constants.STATUS:
                    IsServerRunnning(_config);
                    break;
                default:
                    return;
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

        private void StartServer(ServerConfig serverConfig)
        {
            if (!IsServerRunnning(serverConfig))
            {
                ExecuteCommand(serverConfig.ServerRunCommand, serverConfig);
            }
        }

        private void StopServer(ServerConfig serverConfig)
        {
            ExecuteCommand($"taskkill /IM {serverConfig.ProcessName} /F", serverConfig);
        }

        private void ExecuteCommand(string command, ServerConfig serverConfig)
        {
            Process p = new Process();
            System.Security.SecureString ssPwd = new System.Security.SecureString();
            ProcessStartInfo info = new ProcessStartInfo(@"C:\Windows\System32\cmd.exe");
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;

            info.Domain = serverConfig.Domain;
            info.Verb = "runas";
            info.UserName = serverConfig.UserName;
            string password = serverConfig.Password;
            for (int x = 0; x < password.Length; x++)
            {
                ssPwd.AppendChar(password[x]);
            }
            info.Password = ssPwd;

            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                sw.WriteLine(command);
            }
        }
    }
}
