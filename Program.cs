using System;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Collections.Generic;

namespace Zoltu.PingToInsights
{
	class Program
	{
		static async Task Main(String[] args)
		{
			var destinationIp = Environment.GetEnvironmentVariable("TARGET_IP");
			if (destinationIp == null) throw new Exception("TARGET_IP environment variable must be set.");
			var insightsKey = Environment.GetEnvironmentVariable("ApplicationInsights:InstrumentationKey");
			if (insightsKey == null) throw new Exception("ApplicationInsights:InstrumentationKey variable must be set.");

			var pinger = new Ping();
			var telemetryClient = new TelemetryClient();
			telemetryClient.InstrumentationKey = insightsKey;

			while (true)
			{
				Console.WriteLine("Pinging host.");
				var reply = await pinger.SendPingAsync(destinationIp);
				var properties = new Dictionary<String, String>() { };
				var metrics = new Dictionary<String, Double>()
				{
					["success"] = (reply.Status == IPStatus.Success) ? 1 : 0,
					["status"] = (Int32)reply.Status,
					["duration"] = reply.RoundtripTime,
				};
				telemetryClient.TrackEvent("ping", properties, metrics);
				await Task.Delay(5 * 1000);
			}
		}
	}
}
