using Application.Common.Models;
using Domain.Entities;

namespace Application.Common.Interfaces.Authentication
{
    public interface IAuthRepository
    {
        Task<Result_VM<User>> Authenticate(string email,string password);
    }
}
