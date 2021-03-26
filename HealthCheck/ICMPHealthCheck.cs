using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private readonly string Host;
        private readonly int HealthyRoundtripTime;

        public ICMPHealthCheck(string host, int healthyRoundtripTime)
        {
            Host = host;
            HealthyRoundtripTime = healthyRoundtripTime;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(Host);

                switch (reply.Status)
                {
                    case IPStatus.Success:
                        var msg = $"ICMP to {Host} took {reply.RoundtripTime} ms.";

                        return (reply.RoundtripTime > HealthyRoundtripTime)
                            ? HealthCheckResult.Degraded(msg)
                            : HealthCheckResult.Healthy(msg);

                    default:
                        var err = $"ICMP to {Host} failed: {reply.Status}";
                        return HealthCheckResult.Unhealthy(err);
                }
            }
            catch (Exception ex)
            {
                var err = $"ICMP to {Host} failed: {ex.Message}";
                return HealthCheckResult.Unhealthy(err);
            }
        }
    }
}