using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.BackEnd.Common.Configuration
{
    public class PortfolioServiceCollection : IPortfolioServiceCollection
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _serviceCollection;
        private readonly PortfolioConfiguration _portfolioConfiguration;

        public PortfolioServiceCollection(IServiceCollection serviceCollection, IConfiguration configuration, Action<PortfolioConfiguration> configure)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));            
            _portfolioConfiguration = BuildConfiguration();
            configure?.Invoke(_portfolioConfiguration);
        }

        public IServiceCollection ServiceCollection => _serviceCollection;

        public PortfolioConfiguration PortfolioConfiguration => _portfolioConfiguration;

        public IConfiguration Configuration => _configuration;

        /// <summary>
        /// build portfolio configuration 
        /// </summary>
        /// <returns></returns>
        private PortfolioConfiguration BuildConfiguration()
        {
            var portfolioConfiguration = new PortfolioConfiguration();
            _configuration.GetSection("Portfolio").Bind(portfolioConfiguration);

            _serviceCollection
               .AddSingleton(portfolioConfiguration);


            return portfolioConfiguration;
        }
    }
}
