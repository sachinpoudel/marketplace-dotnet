namespace MarketPlace.Application.Features.Categories.Dtos;















public record CategoryDetailDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId
);