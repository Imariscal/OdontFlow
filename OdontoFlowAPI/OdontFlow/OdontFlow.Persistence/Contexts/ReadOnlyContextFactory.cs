using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;  
using System.IO;

namespace OdontFlow.Persistence.Contexts
{
    public class ReadOnlyContextFactory : IDesignTimeDbContextFactory<ReadOnlyContext>
    {
        public ReadOnlyContext CreateDbContext(string[] args)
        {
            // Busca el appsettings.json desde el proyecto actual
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // O ajusta con ruta del API si es otro proyecto
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ReadOnlyContext>();
            var connectionString = configuration.GetConnectionString("ReadOnly");

            optionsBuilder.UseSqlServer(connectionString);

            return new ReadOnlyContext(optionsBuilder.Options);
        }
    }
}
