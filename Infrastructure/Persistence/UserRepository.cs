using Application.Common.Interfaces.Users;
using Application.Common.Models;
using Dapper;
using Domain.Entities;
using Infrastructure.DbContext;
using System.Data;

namespace Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Result_VM<User>> GetUserByEmail(string email)
    {
        using (var db = _context.CreateConnection())
        {
            try
            {
                var result = await db.QueryFirstOrDefaultAsync<User>("spGetUserByEmail",
                    new { email },
                    commandType: CommandType.StoredProcedure
                    );
                return new()
                {
                    Code = 0,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -1,
                    Message = $"User with this mail not found {ex}"
                };
            }
        }
    }

    public async Task<Result_VM<User>> GetUserById(int id)
    {
        using (var db = _context.CreateConnection())
        {
            try
            {
                var result = await db.QueryFirstOrDefaultAsync<User>("spGetUserById",
                    new { id },
                    commandType: CommandType.StoredProcedure
                    );
                return new()
                {
                    Code = 0,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -1,
                    Message = $"User with this id not found {ex}"
                };
            }
        }
    }

    public async Task<Result_VM<IEnumerable<User>>> GetUsers()
    {
        using (var db = _context.CreateConnection())
        {
            try
            {
                var result = await db.QueryAsync<User>("spGetAllUsers",
                    commandType: CommandType.StoredProcedure
                    );
                return new()
                {
                    Code = 0,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Code = -1,
                    Message = $"There is no user found. {ex}"
                };
            }
        }
    }
}

