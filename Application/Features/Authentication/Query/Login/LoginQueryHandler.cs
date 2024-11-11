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
    private readonly IAuthRepository _authRepository;

    public LoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IAuthRepository authRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _authRepository = authRepository;
    }

    public async Task<Result_VM<string>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var hashedPassword = Hash.HashPassword(query.Password);
        var user = await _authRepository.Authenticate(query.Email,hashedPassword);
        if (user.Result is null)
        {
            return new()
            {
                Code = -1,
                Message = user.Message,
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