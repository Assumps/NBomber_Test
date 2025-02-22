using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Plugins.Network.Ping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBomber_LoadTest.WheatherForecast
{
    public class WeatherForecast
    {
        public void Run()
        {
            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("GetWeatherForecast", async context =>
            {
                var request = Http.CreateRequest("GET", "https://localhost:32769/weatherforecast")
                    .WithHeader("Content-Type", "application/json");

                var response = await Http.Send(httpClient, request);

                return response;
            }).WithoutWarmUp()
            .WithLoadSimulations(
              Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1),during: TimeSpan.FromMinutes(1)),
              Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1)),
              Simulation.RampingInject(rate: 0, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(1))
            );

            NBomberRunner.RegisterScenarios(scenario)
                .WithWorkerPlugins(
                new PingPlugin(PingPluginConfig.CreateDefault("localhost:32769/weatherforecast")),
                new HttpMetricsPlugin(new[] { HttpVersion.Version1 }))
                .Run();
        }

      
    }
}
