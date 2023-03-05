using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Portfolio.BackEnd.Common.Configuration
{
    public interface IPortfolioServiceCollection
    {
        IServiceCollection ServiceCollection { get; }
        PortfolioConfiguration PortfolioConfiguration { get; }
        IConfiguration Configuration { get; }
    }
}
