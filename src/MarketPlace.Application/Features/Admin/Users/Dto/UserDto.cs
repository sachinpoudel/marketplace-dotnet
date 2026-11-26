namespace MarketPlace.Application.Features.Admin.Users.Dto;

public record class UserDto
(
    Guid Id,
    string UserName,
    string Email,
    string Role,
    bool IsActive
);
