using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Users;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
namespace Application.Features.Users.Query.GetUsers;

public class GetUsersQueryHandler :
    IRequestHandler<GetUsersQuery, Result_VM<List<User>>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result_VM<List<User>>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsers();
        if (users.Result is null)
        {
            return new()
            {
                Code = -1,
                Message = "There is no user found",
            };
        }
        return new()
        {
            Code = 0,
            Result = users.Result.ToList()
        };
    }

}