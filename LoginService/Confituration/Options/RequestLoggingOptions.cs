namespace LoginService.Confituration.Options
{
    public class RequestLoggingOptions
    {
        public const string Section = "RequestLogging";

        public int SlowRequestThresholdMs { get; set; } = 1000;

        public List<string> NoisePaths { get; set; } = new List<string>();
    }
}
