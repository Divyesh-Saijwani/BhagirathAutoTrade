using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    public sealed class MasterDataService : IMasterService
    {
        private readonly IRepositoryManager _repositoryManager;

        public MasterDataService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;
    }
}
