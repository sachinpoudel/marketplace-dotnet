using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Admin.Users.Dto;
using MediatR;

namespace MarketPlace.Application.Features.Admin.Users.Command;

public record class UserQuery
(
int PageNumber = 1,
int PageSize = 10
) : IRequest<PaginatedList<UserDto>>;
