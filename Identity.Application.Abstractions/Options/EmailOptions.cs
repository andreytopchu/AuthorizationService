namespace Identity.Application.Abstractions.Options
{
    public class EmailOptions
    {
        public string? FromEmail { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
        public int AttemptCount { get; set; }
        public string? From { get; set; }
        public int ConnectTimeout { get; set; } = 2000;
        public int SendTimeout { get; set; } = 10000;
    }
}