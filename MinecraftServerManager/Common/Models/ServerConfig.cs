namespace Common.Models
{
    public class ServerConfig
    {
        public string ServerRunCommand { get; set; }
        public string Secret { get; set; }
        public string ProcessName { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PubSubConnectionString { get; set; }

    }
}
