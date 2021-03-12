using Steeltoe.CloudFoundry.Connector.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wavefront_sdk
{
    public class WavefrontProxyServiceInfo : ServiceInfo
    {
        public WavefrontProxyServiceInfo(string id, string hostname)
            :base(id)
        {
            
        }

        public WavefrontProxyServiceInfo(string id, string hostname, int port, int distributionPort, int tracingPort)
            :base(id)
        {

        }
    }
}
