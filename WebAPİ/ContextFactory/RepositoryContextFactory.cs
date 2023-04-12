using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repositories;

namespace WebAPİ.ContextFactory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var dbContextOptions = new DbContextOptionsBuilder<RepositoryContext>().UseSqlServer(configuration.GetConnectionString("ConStr"), opt =>
            {
                opt.MigrationsAssembly("WebAPİ");
            });

            return new RepositoryContext(dbContextOptions.Options);
        }
    }
}
