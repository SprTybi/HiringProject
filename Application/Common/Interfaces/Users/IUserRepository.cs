using Application.Common.Models;
using Domain.Entities;

namespace Application.Common.Interfaces.Users
{
    public interface IUserRepository
    {
        Task<Result_VM<User>> GetUserByEmail(string email);
        Task<Result_VM<IEnumerable<User>>> GetUsers();
    }
}
