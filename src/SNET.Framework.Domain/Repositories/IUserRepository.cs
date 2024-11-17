using SNET.Framework.Domain.Entities;

namespace SNET.Framework.Domain.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByIdWithRoles(Guid userId);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid userId);
    }
}
