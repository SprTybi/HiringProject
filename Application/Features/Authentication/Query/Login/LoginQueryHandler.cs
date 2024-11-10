using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Users;
using Application.Common.Models;
using Application.Common.Services;
using MediatR;
namespace Application.Features.Authentication.Query.Login;

public class LoginQueryHandler :
    IRequestHandler<LoginQuery, Result_VM<string>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public LoginQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result_VM<string>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(query.Email);
        if (user.Result is null)
        {
            return new()
            {
                Code = -1,
                Message = "User not found",
            };
        }
        var hashedPassword = Hash.HashPassword(query.Password);
        if (user.Result.PasswordHash != hashedPassword)
        {
            return new()
            {
                Code = -1,
                Message = "Username or Password is wrong",
            };
        }
        var token = _jwtTokenGenerator.GenerateToken(user.Result);
        return new()
        {
            Code = 0,
            Result = token
        };
    }

}