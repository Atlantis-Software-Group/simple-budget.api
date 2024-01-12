using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using simple_budget.api.data;
using simple_budget.api.data.Transactions;
using simple_budget.api.interfaces;
using simple_budget.api.Services;

namespace simple_budget.api;

public static class HostingExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddControllers();
        services.AddHttpClient();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
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

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, (options) =>
        {
            options.Authority = configuration["Authentication:Schemes:Bearer:Authority"];
            options.MapInboundClaims = false;
            options.RequireHttpsMetadata = true;            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidTypes = new[] { "at+jwt" },
                ValidIssuer = configuration["Authentication:Schemes:Bearer:ValidIssuer"],
                ValidateLifetime = true
            };

            options.Events = new JwtBearerEvents {
                OnTokenValidated = JwtEventHandlers.OnJwtTokenValidated
            };
        });

        var TransactionDbConnectionString = configuration.GetConnectionString("TransactionDb");

        services.AddDbContext<ApplicationDbContext>(options =>
        {                    
            options.UseSqlServer(TransactionDbConnectionString);            
        });

        services.AddSingleton(TimeProvider.System);
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped<IGetUserInfoService, GetUserInfoService>();
        services.AddScoped<IClaimsTransformation, ApiClaimsTranformerService>();
        services.AddHealthChecks();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    public static ConfigureHostBuilder ConfigureHost(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            IConfiguration config = context.Configuration;
            string seqUrl = config["Seq:ServerUrl"] ?? string.Empty;

            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Information)
                .Enrich.WithProperty("Application", "SimpleBudget.API")
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .WriteTo.Seq(seqUrl)
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
        });
        return host;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();
        app.UseHealthChecks("/health");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers()
            .RequireAuthorization();

        return app;
    }
}
