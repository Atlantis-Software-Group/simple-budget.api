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
IdentityModelEventSource.ShowPII = true;
builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        {
            new OpenApiSecurityScheme() {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, (options) =>
                {
                    options.Authority = builder.Configuration["Authentication:Schemes:Bearer:Authority"];
                    options.MapInboundClaims = false;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ValidTypes = new[] { "at+jwt" },
                        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"],
                        ValidateLifetime = true
                    };
                    options.Events = new JwtBearerEvents() {
                        OnTokenValidated = JwtEventHandlers.OnJwtTokenValidated
                    };
                });

builder.Services.AddHttpClient();
builder.Services.AddTransient<IGetUserInfoService, GetUserInfoService>();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

try
{
    IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    Serilog.Debugging.SelfLog.Enable(Console.Error);
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
