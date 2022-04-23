using AesGcmTest.Domain;
using AesGcmTest.Infrastructure;
using MediatR;

namespace AesGcmTest.Web.Hosting;

public class Startup
{
    private readonly IConfiguration _configuration;
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddInfrastructure(_configuration)
            .AddMediatR(typeof(Startup).Assembly)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddControllers();

    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .InitializeDatabase()
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
