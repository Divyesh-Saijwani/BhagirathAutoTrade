namespace Identity.Core.Security.Tokens
{
    public class RefreshToken : JsonWebToken
    {
        public RefreshToken(string token, long expiration) : base(token, expiration)
        {
        }
    }

    public class SecurityVerificationToken : JsonWebToken
    {
        public SecurityVerificationToken(string token, long expiration) : base(token, expiration)
        {
        }
    }
}