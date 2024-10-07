namespace Identity.Core.Security.Tokens
{
	public class RefreshTokenWithEmail
	{
		public string Email { get; set; }
		public RefreshToken RefreshToken { get; set; }
	}

	public class EmailVerificationToken
	{
        public string Email { get; set; }
        public SecurityVerificationToken SecurityToken { get; set; }
    }
}
