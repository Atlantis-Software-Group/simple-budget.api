using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using simple_budget.api;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureTokenValidationParameters>();
// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);
builder.Host.ConfigureHost();

var app = builder.Build();

app.ConfigurePipeline();
try
{       
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
