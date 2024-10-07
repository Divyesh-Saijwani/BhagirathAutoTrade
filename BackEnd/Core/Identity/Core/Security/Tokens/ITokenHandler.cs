using Identity.Core.Models;

namespace Identity.Core.Security.Tokens
{
    public interface ITokenHandler
    {
        AccessToken CreateAccessToken(User user);
        RefreshToken TakeRefreshToken(string token, string userEmail);
        Tuple<bool, string> VerifySecurityToken(string token, string userEmail);
        SecurityVerificationToken GetSecurityVerificationToken(string userEmail);
        void RevokeRefreshToken(string token, string userEmail);
    }
}