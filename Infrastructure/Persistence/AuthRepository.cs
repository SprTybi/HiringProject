using Application.Common.Interfaces.Authentication;
using Application.Common.Models;
using Dapper;
using Domain.Entities;
using Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AuthRepository:IAuthRepository
    {
        private readonly DapperContext _context;

        public AuthRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Result_VM<User>> Authenticate(string email,string password)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<User>("spAuthenticate",
                        new { email , password },
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
                        Message = $"Username or Password is wrong.{ex}"
                    };
                }
            }
        }
    }

}
