using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.BackEnd.Common.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IPortfolioServiceCollection AddPortfolio(this IServiceCollection serviceCollection,
            IConfiguration configuration, Action<PortfolioConfiguration> configure = null)
        {
            return new PortfolioServiceCollection(serviceCollection, configuration, configure);
        }
    }
}
