using Identity.Core.Repositories;
using Identity.Core.Services;
using Identity.Core.Models;

namespace Identity.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Role> FindByNameAsync(string role)
        {
            return await _roleRepository.FindByNameAsync(role);
        }
    }
}
