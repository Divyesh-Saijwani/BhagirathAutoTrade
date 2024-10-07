using Identity.Services.Communication;

namespace Identity.Core.Models
{
    public class EmailVerificationResponse : BaseResponse
    {
        public EmailVerificationResponse(bool success, string message) : base(success, message)
        {
        }
    }
}
