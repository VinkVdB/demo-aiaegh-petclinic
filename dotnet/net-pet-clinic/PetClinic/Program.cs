using Microsoft.EntityFrameworkCore;
using PetClinic.Data;
using PetClinic.Data.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
var mvcBuilder = builder.Services.AddControllersWithViews(options =>
{
    // Enable automatic antiforgery validation for POST requests
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

#if DEBUG
// Add runtime compilation for development hot-reload
mvcBuilder.AddRazorRuntimeCompilation();
#endif

// Configure Entity Framework with SQLite
builder.Services.AddDbContext<PetClinicContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository pattern registration
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IVetRepository, VetRepository>();
builder.Services.AddScoped<IPetTypeRepository, PetTypeRepository>();

// API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PetClinic API",
        Version = "v1",
        Description = "A sample .NET Core API for the PetClinic application",
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0")
        }
    });
    
    // Only include API controllers (with [ApiController] attribute) in Swagger
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        var controllerType = apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<Microsoft.AspNetCore.Mvc.ControllerAttribute>()
            .FirstOrDefault()?.GetType().DeclaringType;
        
        return apiDesc.ActionDescriptor.EndpointMetadata
            .Any(x => x is Microsoft.AspNetCore.Mvc.ApiControllerAttribute);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetClinic API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();     // remains stubbed-out (no auth)

// Health check endpoint for Docker
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Database initialization
app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PetClinicContext>();
    await ApplicationDbInitializer.SeedAsync(context);
});

app.Run();

// Make the Program class accessible to integration tests
public partial class Program { }
