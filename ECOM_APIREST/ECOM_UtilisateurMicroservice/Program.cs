using Microsoft.OpenApi.Models;
using ECOM_UtilisateurMicroservice.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient<DummyJsonService>();
builder.Services.AddScoped<DummyJsonService>();
builder.Services.AddScoped<DbInitializer>();

builder.Services.AddDbContext<ECOM_UtilisateurMicroservice.UtilisateurDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Utilisateur Microservice - RestAPI",
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var dbInitializer = services.GetRequiredService<DbInitializer>();
        await dbInitializer.InitializeDatabaseAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Utilisateur Microservice - RestAPI V1.0");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();