using Application.Common.Models;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Query.GetUsers;

public record GetUsersQuery() : IRequest<Result_VM<List<User>>>;

