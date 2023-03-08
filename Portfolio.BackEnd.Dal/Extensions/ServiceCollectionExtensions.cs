using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Portfolio.BackEnd.Common.Configuration;
using Portfolio.BackEnd.Dal.Auth;
using Portfolio.BackEnd.Dal.Configuration;
using System.Text;

namespace Portfolio.BackEnd.Dal.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IPortfolioServiceCollection AddIdentityService(this IPortfolioServiceCollection portfolioServiceCollection)
        {
            var portfolioIdentityConfiguration = BuildConfiguration(portfolioServiceCollection);
            portfolioServiceCollection.ServiceCollection.AddSingleton(portfolioIdentityConfiguration);

            portfolioServiceCollection.ServiceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(portfolioIdentityConfiguration.SqlServerConnectionString));

            portfolioServiceCollection.ServiceCollection.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            var eshopDecryptHelper = new PortfolioDecryptHelper(portfolioIdentityConfiguration.Secret);

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
                     ValidAudience = portfolioIdentityConfiguration.ValidAudience,
                     ValidIssuer = portfolioIdentityConfiguration.ValidIssuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(portfolioIdentityConfiguration.Secret))
                 };

                 options.SecurityTokenValidators.Clear();
                 options.SecurityTokenValidators.Add(new PortfolioSecurityTokenValidator(eshopDecryptHelper));
             });

            portfolioServiceCollection.ServiceCollection.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Bearer")
                    .Build();

                var approvedPolicyBuilder = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Bearer");

                options.AddPolicy("approved", approvedPolicyBuilder.Build());
            });

            portfolioServiceCollection.ServiceCollection.AddScoped<IAuthManagerService, AuthManagerService>();

            return portfolioServiceCollection;
        }

        private static PortfolioIdentityConfiguration BuildConfiguration(IPortfolioServiceCollection portfolioServiceCollection)
        {
            var portfolioConfiguration = new PortfolioIdentityConfiguration();
            portfolioServiceCollection.Configuration.GetSection("Portfolio:JWT").Bind(portfolioConfiguration);

            return portfolioConfiguration;
        }
    }
}
