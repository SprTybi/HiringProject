using Application.Common.Models;
using MediatR;

namespace Application.Features.Authentication.Query.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<Result_VM<string>>;

