using Domain.Repositories;

namespace Persistance.Repositories
{
    public class MasterRepository : IMasterRepository
    {
        private readonly RepositoryDbContext _repositoryDbContext;

        public MasterRepository(RepositoryDbContext repositoryDbContext) => _repositoryDbContext = repositoryDbContext;
    }
}
