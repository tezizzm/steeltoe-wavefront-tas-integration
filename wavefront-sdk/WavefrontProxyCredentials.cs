namespace wavefront_sdk
{
    public class WavefrontProxyCredentials
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public int DistributionPort { get; set; }
        public int TracingPort { get; set; }
    }
}
