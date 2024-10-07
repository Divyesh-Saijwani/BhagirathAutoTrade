using Identity.Core.Repositories;

namespace Identity.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;

        public UnitOfWork(AuthDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}