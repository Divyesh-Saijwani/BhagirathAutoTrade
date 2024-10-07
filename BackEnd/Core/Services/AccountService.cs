using Contracts;
using Services.Abstractions;
using Domain.Repositories;
using Identity.Services;
using AlgoBhagirath.Common.Enums;
using Identity.Security;
using Microsoft.AspNetCore.Http;
using Mapster;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IUserService _userService;
        public AccountService(IRepositoryManager repositoryManager, IUserService userService)
        {
            _repositoryManager = repositoryManager;
            _userService = userService;
        }
        public async Task<UserResponseDTO?> GetIdentityUserData(string email)
        {
            var user = await _userService.FindByEmailAsync(email);
            return user.Adapt< UserResponseDTO>();
        }
    }
}
