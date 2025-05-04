using VirtuoInventory.Core.Entities;

namespace VirtuoInventory.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> AuthenticateUser(string username);
        Task<int> InsertUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> UpdateUserPassword(int id, string newPassword);
        Task<bool> DeleteUser(int id);


    }
}