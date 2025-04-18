using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Mappers;
using GAC.WMS.Integration.Application.Services;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.FileProcessor;
using GAC.WMS.Integration.Infrastructure;
using GAC.WMS.Integration.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Database Configuration
        services.AddDbContext<IntegrationDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

        // Services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<ISalesOrderService, SalesOrderService>();

        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        // File Processor
        services.AddHostedService<FileProcessorService>();

        // Create required directories
        var watchPath = hostContext.Configuration["FileProcessor:WatchPath"];
        var processedPath = hostContext.Configuration["FileProcessor:ProcessedPath"];
        var errorPath = hostContext.Configuration["FileProcessor:ErrorPath"];

        Directory.CreateDirectory(watchPath);
        Directory.CreateDirectory(processedPath);
        Directory.CreateDirectory(errorPath);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

// Ensure database is created
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();
    dbContext.Database.Migrate();
}

await host.RunAsync();
