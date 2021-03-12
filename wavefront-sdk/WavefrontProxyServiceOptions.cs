using Microsoft.Extensions.Configuration;

using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace wavefront_sdk
{
    public class WavefrontProxyServiceOptions : CloudFoundryServicesOptions
    {
        public WavefrontProxyServiceOptions() { }
        public WavefrontProxyServiceOptions(IConfiguration config) : base(config) { }
        public WavefrontProxyCredentials Credentials { get; set; }
    }
}
