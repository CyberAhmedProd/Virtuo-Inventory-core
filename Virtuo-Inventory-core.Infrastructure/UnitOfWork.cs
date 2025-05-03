using VirtuoInventory.Application.Interfaces;

namespace VirtuoInventory.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IUserRepository userRepository)
        {
            Users = userRepository;
        }

        public IUserRepository Users { get; set; }
    }
}