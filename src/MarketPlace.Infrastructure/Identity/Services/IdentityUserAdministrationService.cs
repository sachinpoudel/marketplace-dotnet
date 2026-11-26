using System;
using MarketPlace.Application.Common.Interfaces.Admin;
using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Admin.Users.Command;
using MarketPlace.Application.Features.Admin.Users.Command.ChangeUserRole;
using MarketPlace.Application.Features.Admin.Users.Command.SetUserStatus;
using MarketPlace.Application.Features.Admin.Users.Dto;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Infrastructure.Identity.Services;

public class IdentityUserAdministrationService : Application.Common.Interfaces.Admin.IUserAdministrationService
{
    public Task<Result> ChangeUserRoleAsync(ChangeUserRoleCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PaginatedList<UserDto>>> GetUsersAsync(UserQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SetUserStatusAsync(SetUserStatusCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
