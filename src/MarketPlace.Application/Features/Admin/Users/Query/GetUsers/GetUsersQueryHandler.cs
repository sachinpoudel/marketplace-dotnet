using System;
using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Admin.Users.Command;
using MarketPlace.Application.Features.Admin.Users.Dto;
using MediatR;

namespace MarketPlace.Application.Features.Admin.Users.Query.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<UserQuery, PaginatedList<UserDto>>
{
    public Task<PaginatedList<UserDto>> Handle(UserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
