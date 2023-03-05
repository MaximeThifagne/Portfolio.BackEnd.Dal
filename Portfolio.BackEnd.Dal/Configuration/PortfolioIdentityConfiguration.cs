using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.BackEnd.Dal.Configuration
{
    public class PortfolioIdentityConfiguration
    {
        public string SqlServerConnectionString { get; set; }

        public string Secret { get; set; }

        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }
    }
}
