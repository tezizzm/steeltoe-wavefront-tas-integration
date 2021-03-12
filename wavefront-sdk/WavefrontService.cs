using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Steeltoe.Extensions.Configuration.CloudFoundry;

using Wavefront.AspNetCore.SDK.CSharp.Common;
using Wavefront.OpenTracing.SDK.CSharp;
using Wavefront.OpenTracing.SDK.CSharp.Reporting;
using Wavefront.SDK.CSharp.Common.Application;
using Wavefront.SDK.CSharp.Proxy;

using OpenTracing;
using Wavefront.SDK.CSharp.DirectIngestion;

namespace wavefront_sdk
{
    public static class SteeltoeWavefrontProxyExtensions
    {
        public static IServiceCollection AddSteeltoeWavefrontProxy(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var sp = services.BuildServiceProvider();

            var config = sp.GetRequiredService<IConfiguration>();

            System.Console.WriteLine(config);
            var cloudFoundryApplicationOptions = new CloudFoundryApplicationOptions(config);
            var waveFrontProxyOptions = new WavefrontProxyServiceOptions(config);
            waveFrontProxyOptions.Credentials = new WavefrontProxyCredentials
            {
                DistributionPort = 2878,
                Hostname = "192.168.8.20",
                Port = 2878,
                TracingPort = 30000
            };

            string application = $"{cloudFoundryApplicationOptions.SpaceName}.{cloudFoundryApplicationOptions.ApplicationName}.WavefrontProxy";
            string service = $"{application}.{cloudFoundryApplicationOptions.InstanceId}";
            string cluster = $"Host: {cloudFoundryApplicationOptions.InstanceIP}";
            string shard = $"Container: {cloudFoundryApplicationOptions.InternalIP}";

            var applicationTags = new ApplicationTags.Builder(application, service)
              .Cluster(cluster)
              .Shard(shard)
              .Build();


            var wfProxyClientBuilder = new WavefrontProxyClient.Builder(waveFrontProxyOptions.Credentials.Hostname);
            wfProxyClientBuilder.MetricsPort(waveFrontProxyOptions.Credentials.Port);
            wfProxyClientBuilder.DistributionPort(waveFrontProxyOptions.Credentials.DistributionPort);
            wfProxyClientBuilder.TracingPort(waveFrontProxyOptions.Credentials.TracingPort);
            wfProxyClientBuilder.FlushIntervalSeconds(2);
            var wavefrontSender = wfProxyClientBuilder.Build();

            System.Console.WriteLine(wavefrontSender);


            var wfAspNetCoreReporter = new WavefrontAspNetCoreReporter.Builder(applicationTags)
                .WithSource("mySource")
                .ReportingIntervalSeconds(30)
                .Build(wavefrontSender);

            System.Console.WriteLine(wfAspNetCoreReporter);

            var wavefrontSpanReporter = new WavefrontSpanReporter.Builder()
              .Build(wavefrontSender);

            System.Console.WriteLine(wavefrontSpanReporter);

            ITracer tracer = new WavefrontTracer.Builder(wavefrontSpanReporter, applicationTags).Build();

            System.Console.WriteLine(tracer);

            services.AddWavefrontForMvc(wfAspNetCoreReporter, tracer);

            return services;
        }

        public static IServiceCollection AddSteeltoeWavefrontDirectIngestion(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var sp = services.BuildServiceProvider();

            var config = sp.GetRequiredService<IConfiguration>();

            System.Console.WriteLine(config);
            var cloudFoundryApplicationOptions = new CloudFoundryApplicationOptions(config);
            var waveFrontProxyOptions = new WavefrontProxyServiceOptions(config);
            waveFrontProxyOptions.Credentials = new WavefrontProxyCredentials
            {
                DistributionPort = 2878,
                Hostname = "https://demo.wavefront.com/",
                Port = 2878,
                TracingPort = 30000
            };


            string application = $"SDK.Testing.DirectionIngestion";
            string service = $"SDK.Testing.DirectionIngestion.Service";
            string cluster = $"PCF One - TAS";
            string shard = $"PCF One - TAS Shard";

            var applicationTags = new ApplicationTags.Builder(application, service)
              .Cluster(cluster)
              .Shard(shard)
              .Build();


            var wfDirectIngestionClientBuilder = new WavefrontDirectIngestionClient.Builder(waveFrontProxyOptions.Credentials.Hostname, "TOKEN GOES HERE");
            wfDirectIngestionClientBuilder.MaxQueueSize(100_000);
            wfDirectIngestionClientBuilder.BatchSize(20_000);
            wfDirectIngestionClientBuilder.FlushIntervalSeconds(2);
            var wavefrontSender = wfDirectIngestionClientBuilder.Build();


            var wfAspNetCoreReporter = new WavefrontAspNetCoreReporter.Builder(applicationTags)
                .WithSource("mySource")
                .ReportingIntervalSeconds(30)
                .Build(wavefrontSender);


            var wavefrontSpanReporter = new WavefrontSpanReporter.Builder()
              .Build(wavefrontSender);

            System.Console.WriteLine(wavefrontSpanReporter);

            ITracer tracer = new WavefrontTracer.Builder(wavefrontSpanReporter, applicationTags).Build();

            System.Console.WriteLine(tracer);

            services.AddWavefrontForMvc(wfAspNetCoreReporter, tracer);

            return services;
        }
    }
}
