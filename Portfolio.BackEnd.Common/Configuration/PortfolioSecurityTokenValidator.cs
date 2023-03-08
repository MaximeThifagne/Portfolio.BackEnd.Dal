using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.BackEnd.Common.Configuration
{
    public class PortfolioSecurityTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly PortfolioDecryptHelper _portfolioDecryptHelper;

        public PortfolioSecurityTokenValidator(PortfolioDecryptHelper portfolioDecryptHelper)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _portfolioDecryptHelper = portfolioDecryptHelper;
        }

        public bool CanValidateToken(string securityToken) =>
             _tokenHandler.CanReadToken(securityToken);

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
        bool ISecurityTokenValidator.CanValidateToken => true;

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
                var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out var v);
                validatedToken = _tokenHandler.ReadJwtToken(securityToken);
                return BuildClaimsPrincipal(principal, (JwtSecurityToken)validatedToken);
        }

        private ClaimsPrincipal BuildClaimsPrincipal(IPrincipal principal, JwtSecurityToken securityToken)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            if (securityToken == null) throw new ArgumentNullException(nameof(securityToken));

            var claims = new List<Claim>(securityToken.Claims);
            //foreach (var claim in ClaimsToDecrypt)
            //{
            //    Decrypt(claim, claims);
            //}

            if (principal.Identity != null)
            {
                var identity = new ClaimsIdentity(claims, principal.Identity.AuthenticationType);
                return new ClaimsPrincipal(identity);
            }
            return null;
        }

        private void Decrypt(string type, ICollection<Claim> claims)
        {
            var claim = claims.FirstOrDefault(c => c.Type == type);
            if (claim == null) return;
            var data = _portfolioDecryptHelper.Decrypt(claim.Value);
            claims.Remove(claim);
            claims.Add(new Claim(type, data, claim.ValueType, claim.Issuer));
        }

        public bool CanReadToken(string securityToken) => _tokenHandler.CanReadToken(securityToken);
    }
}
