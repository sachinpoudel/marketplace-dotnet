using System;
using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Admin.Users.Command;
using MarketPlace.Application.Features.Admin.Users.Command.ChangeUserRole;
using MarketPlace.Application.Features.Admin.Users.Command.SetUserStatus;
using MarketPlace.Application.Features.Admin.Users.Dto;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Application.Common.Interfaces.Admin;

public interface IUserAdministrationService
{
  Task<Result<PaginatedList<UserDto>>> GetUsersAsync(UserQuery query, CancellationToken cancellationToken = default);
Task<Result> ChangeUserRoleAsync(ChangeUserRoleCommand command, CancellationToken cancellationToken = default);
Task<Result> SetUserStatusAsync(SetUserStatusCommand command, CancellationToken cancellationToken = default);

}
