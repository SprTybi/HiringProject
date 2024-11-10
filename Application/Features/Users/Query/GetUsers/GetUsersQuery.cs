using Application.Common.Models;
using Domain.Entities;
using MediatR;

namespace Application.Features.Authentication.Query.Login;

public record GetUsersQuery() : IRequest<Result_VM<List<User>>>;

