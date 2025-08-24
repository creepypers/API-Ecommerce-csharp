using MMLib.SwaggerForOcelot.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(option => option.EnableEndpointRouting = false);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var routes = "Routes";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ECOM_AuthentificationMicroservice";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ECOM_API_Clients";
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? 
    "ECOM_tp_suPer_Secret_Key_For_JWT_Auth_2024_Default";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("APIGatewayAuthenticationScheme", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMvc();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseOcelot().Wait();

app.Run();
