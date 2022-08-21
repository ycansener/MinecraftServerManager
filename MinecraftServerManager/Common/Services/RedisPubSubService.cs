using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class RedisPubSubService
    {
        private readonly string _connectionString;
        private IConnectionMultiplexer? _connection;
        private bool _terminated;

        public RedisPubSubService(string connectionString)
        {
            _connectionString = connectionString;
            _connection = null;
            _terminated = true;
        }

        public async Task SubscribeAsync(string channelName, Action<string, string> messageHandler)
        {
            var connection = await GetConnectionAsync();
            var subscriber = connection.GetSubscriber(channelName);

            await subscriber.SubscribeAsync(channelName, (RedisChannel channel, RedisValue message) =>
            {
                if(message == Constants.TERMINATE)
                {
                    Terminate();
                    return;
                }

                messageHandler(channel.ToString(), message.ToString());
            });
        }

        public async Task PublishAsync(string channelName, string message)
        {
            var connection = await GetConnectionAsync();
            var subscriber = connection.GetSubscriber(channelName);

            _ = await subscriber.PublishAsync(channelName, message, CommandFlags.FireAndForget);
        }

        private async Task<IConnectionMultiplexer> GetConnectionAsync()
        {
            if(_connection == null || !_connection.IsConnected)
            {
                _connection = await ConnectionMultiplexer.ConnectAsync(_connectionString);
            }

            return _connection;
        }

        public async Task WaitUntilTerminationAsync()
        {
            _terminated = false;
            while (!_terminated)
            {
                await Task.Delay(500);
            }
        }

        public void Terminate()
        {
            _terminated = true;
        }
    }
}
