using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Portfolio.BackEnd.Common.Configuration;
using Portfolio.BackEnd.Dal.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.BackEnd.Dal.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IPortfolioServiceCollection AddIdentityService(this IPortfolioServiceCollection portfolioServiceCollection)
        {
            var portfolioConfiguration = BuildConfiguration(portfolioServiceCollection);
            portfolioServiceCollection.ServiceCollection.AddSingleton(portfolioConfiguration);

            portfolioServiceCollection.ServiceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(portfolioConfiguration.SqlServerConnectionString));

            portfolioServiceCollection.ServiceCollection.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Adding Authentication
            portfolioServiceCollection.ServiceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
             {
                 options.SaveToken = true;
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidAudience = portfolioConfiguration.ValidAudience,
                     ValidIssuer = portfolioConfiguration.ValidIssuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(portfolioConfiguration.Secret))
                 };
             });

            portfolioServiceCollection.ServiceCollection.AddScoped<IAuthManagerService, AuthManagerService>();

            return portfolioServiceCollection;
        }

        private static PortfolioConfiguration BuildConfiguration(IPortfolioServiceCollection portfolioServiceCollection)
        {
            var portfolioConfiguration = new PortfolioConfiguration();
            portfolioServiceCollection.Configuration.GetSection("JWT").Bind(portfolioConfiguration);

            return portfolioConfiguration;
        }
    }
}
