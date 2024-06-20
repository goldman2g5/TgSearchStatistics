using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TgSearchStatistics.Services;

namespace TgSearchStatistics.Models.BaseModels
{
    public class MyDbContextFactory : IDbContextFactory<TgDbContext>
    {
        private readonly DbContextOptions<TgDbContext> _options;
        private readonly IServiceScopeFactory _scopeFactory;

        public MyDbContextFactory(DbContextOptions<TgDbContext> options, IServiceScopeFactory scopeFactory)
        {
            _options = options;
            _scopeFactory = scopeFactory;
        }

        public TgDbContext CreateDbContext()
        {
            return new TgDbContext(_options, _scopeFactory);
        }
    }
}
