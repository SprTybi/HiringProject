using System.Text;
using Infrastructure.DbContext;
using Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Infrastructure.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Application.Common.Interfaces.Users;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.Common.Interfaces.RabbitMQ;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddAuth(configuration)
            .AddContext(configuration)
            .AddPersistence();

        return services;
    }


    public static IServiceCollection AddPersistence(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddSingleton<IRabbitMqService, RabbitMqService>();

        return services;
    }

    public static IServiceCollection AddContext(
        this IServiceCollection services, ConfigurationManager configuration)
    {
        var DapperSetting = new DapperSettings();
        configuration.Bind(DapperSettings.SectionName, DapperSetting);
        services.AddSingleton(Options.Create(DapperSetting));
        services.AddSingleton<DapperContext>();

        return services;
    }

    public static IServiceCollection AddAuth(
    this IServiceCollection services, ConfigurationManager configuration)
    {
        var JwtSetting = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, JwtSetting);

        services.AddSingleton(Options.Create(JwtSetting));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JwtSetting.Issuer,
                    ValidAudience = JwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JwtSetting.Secret))
                };
            });
        return services;
    }
}

