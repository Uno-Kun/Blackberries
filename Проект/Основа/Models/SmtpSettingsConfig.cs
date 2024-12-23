namespace Blackberries.Models
{
    public class SmtpSettingsConfig
    {
        public string Host {  get; set; }

        public int Port { get; set; }

        public bool EnableSSL { get; set; }

        public int Timeout { get; set; }

        public string Login {  get; set; }

        public string Password { get; set; }
    }
}
