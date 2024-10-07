using Contracts;

namespace Services.Abstractions
{
    public interface IAccountService
    {
        Task<UserResponseDTO?> GetIdentityUserData(string email);
    }
}
