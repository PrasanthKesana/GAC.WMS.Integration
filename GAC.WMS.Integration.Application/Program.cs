using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using FluentValidation;
using GAC.WMS.Integration.Application.Extensions;
using GAC.WMS.Integration.Application.Filters;
using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Mappers;
using GAC.WMS.Integration.Application.Middleware;
using GAC.WMS.Integration.Application.Services;
using GAC.WMS.Integration.Application.Validators;
using GAC.WMS.Integration.Domain.Interfaces.Repositories;
using GAC.WMS.Integration.Infrastructure;
using GAC.WMS.Integration.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Database Configuration
builder.Services.AddDbContext<IntegrationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GAC WMS API",
        Version = "v1",
        Description = "Warehouse Management System API",
        Contact = new OpenApiContact
        {
            Name = "Prasanth",
            Email = "test@example.com"
        }
    });

    // Force OpenAPI 3.0 specification
    c.DocumentFilter<ForceOpenApiSpecVersionFilter>();

    // Include XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure retry policies
builder.Services.AddResiliencePolicies(builder.Configuration);


var app = builder.Build();

// Middleware pipeline - UPDATED
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = false; // Explicitly disable Swagger 2.0
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GAC WMS API v1");
        c.RoutePrefix = "swagger";
        c.ConfigObject.AdditionalItems["syntaxHighlight"] = false; 
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IntegrationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

public class ForceOpenApiSpecVersionFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // This is a workaround for newer Swashbuckle versions
        var openApiVersionField = typeof(OpenApiDocument).GetField("_openApiVersion",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (openApiVersionField != null)
        {
            openApiVersionField.SetValue(swaggerDoc, "3.0.1");
        }
    }
}