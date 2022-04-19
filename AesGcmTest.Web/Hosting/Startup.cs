using AesGcmTest.Domain;
using AesGcmTest.Infrastructure;
using MediatR;

namespace AesGcmTest.Web.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddCompletelyInMemoryTenancyEncryptionStorage()
            .AddTransient<IUserRepository, InMemoryUserRepository>()
            .AddMediatR(typeof(Startup).Assembly)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddControllers();

    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            })
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
    }
}
