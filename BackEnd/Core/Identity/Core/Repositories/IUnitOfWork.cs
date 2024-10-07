namespace Identity.Core.Repositories
{
    public interface IUnitOfWork
    {
         Task CompleteAsync();
    }
}