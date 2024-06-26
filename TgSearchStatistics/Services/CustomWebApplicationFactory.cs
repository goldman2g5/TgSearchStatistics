using Microsoft.AspNetCore.Mvc.Testing;
using TgSearchStatistics.Models.BaseModels;


namespace TgSearchStatistics.Services
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TgDbContext>();
                    db.Database.EnsureCreated();
                }
            });

            builder.UseEnvironment("Development");
        }
    }
}
