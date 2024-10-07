using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Core.Models;
using Identity.Core.Security.Hashing;
using Microsoft.Extensions.Options;

namespace Identity.Core.Security.Tokens
{
    public class TokenHandler : ITokenHandler
    {
        private readonly ISet<RefreshTokenWithEmail> _refreshTokens = new HashSet<RefreshTokenWithEmail>();
        private readonly ISet<EmailVerificationToken> _emailVerificationTokens = new HashSet<EmailVerificationToken>();
        private readonly TokenOptions _tokenOptions;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly IPasswordHasher _passwordHaser;

        public TokenHandler(IOptions<TokenOptions> tokenOptionsSnapshot, SigningConfigurations signingConfigurations, IPasswordHasher passwordHaser)
        {
            _passwordHaser = passwordHaser;
            _tokenOptions = tokenOptionsSnapshot.Value;
            _signingConfigurations = signingConfigurations;
        }

        public AccessToken CreateAccessToken(User user)
        {
            var refreshToken = BuildRefreshToken();
            var accessToken = BuildAccessToken(user, refreshToken);
            _refreshTokens.Add(new RefreshTokenWithEmail 
            {
                Email = user.Email,
                RefreshToken = refreshToken,
            });

            return accessToken;
        }

        public RefreshToken TakeRefreshToken(string token, string userEmail)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userEmail))
                return null;

            var refreshTokenWithEmail = _refreshTokens.SingleOrDefault(t => t.RefreshToken.Token == token && t.Email == userEmail);
            if(refreshTokenWithEmail == null)
			{
                return null;
			}
            
            _refreshTokens.Remove(refreshTokenWithEmail);
            return refreshTokenWithEmail.RefreshToken;
        }

        public void RevokeRefreshToken(string token, string userEmail)
        {
            TakeRefreshToken(token, userEmail);
        }

        public Tuple<bool,string> VerifySecurityToken(string token, string userEmail)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userEmail))
                return new Tuple<bool, string>(false, "Token not found");

            var securityTokenWithEmail = _emailVerificationTokens.SingleOrDefault(t => t.SecurityToken.Token == token && t.Email == userEmail);
            
            if (securityTokenWithEmail == null)
            {
                return new Tuple<bool, string>(false, "Token not found");
            }

            if(securityTokenWithEmail.SecurityToken.IsExpired())
            {
                return new Tuple<bool, string>(false, "Email verification token expored");
            }

            _emailVerificationTokens.Remove(securityTokenWithEmail);
            
            return new Tuple<bool, string>(true,"Token verified successfully");
        }

        public SecurityVerificationToken GetSecurityVerificationToken(string userEmail)
        {
            var securityVerificationToken = new SecurityVerificationToken
            (
                token: _passwordHaser.HashPassword(userEmail),
                expiration: DateTime.UtcNow.AddDays(1).Ticks
            );

            _emailVerificationTokens.Add(new EmailVerificationToken { Email=userEmail, SecurityToken = securityVerificationToken });

            return securityVerificationToken;
        }

        private RefreshToken BuildRefreshToken()
        {
            var refreshToken = new RefreshToken
            (
                token : _passwordHaser.HashPassword(Guid.NewGuid().ToString()),
                expiration : DateTime.UtcNow.AddMinutes(_tokenOptions.RefreshTokenExpiration).Ticks
            );

            return refreshToken;
        }

        private AccessToken BuildAccessToken(User user, RefreshToken refreshToken)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);

            var securityToken = new JwtSecurityToken
            (
                issuer : _tokenOptions.Issuer,
                audience : _tokenOptions.Audience,
                claims : GetClaims(user),
                expires : accessTokenExpiration,
                notBefore : DateTime.UtcNow,
                signingCredentials : _signingConfigurations.SigningCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(securityToken);

            return new AccessToken(accessToken, accessTokenExpiration.Ticks, refreshToken);
        }

        private IEnumerable<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name,user.Email)
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            return claims;
        }
    }
}
