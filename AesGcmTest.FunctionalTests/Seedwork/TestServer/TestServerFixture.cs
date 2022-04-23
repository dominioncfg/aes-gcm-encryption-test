using AesGcmTest.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace AesGcmTest.FunctionalTests.Seedwork;

public sealed class TestServerFixture : IDisposable
{
    private static TestServerFixture? FixtureInstance { get; set; }
    public TestServer Server { get; }

    public static void OnTestInitResetApplicationState()
    {
        FixtureInstance!.OnTestInitClearPostgreSqlDb().GetAwaiter().GetResult();
    }

    public TestServerFixture()
    {
        IHostBuilder hostBuilder = ConfigureHost();
        var host = hostBuilder.StartAsync().GetAwaiter().GetResult();
        Server = host.GetTestServer();
        FixtureInstance = this;
    }

    public void Dispose() => Server.Dispose();

    private static IHostBuilder ConfigureHost()
    {
        return new HostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                  builder
                     .SetBasePath(context.HostingEnvironment.ContentRootPath)
                     .AddJsonFile("appsettings.json")
                     .AddUserSecrets(typeof(Startup).GetTypeInfo().Assembly);
            })
            .UseEnvironment("Test")
            .ConfigureServices(services =>
            {

            })
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<Startup>();
            });
    }
}
