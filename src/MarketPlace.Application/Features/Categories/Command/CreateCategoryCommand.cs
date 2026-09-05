using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Categories.Command;

public record CreateCategoryCommand(string Name, string Description, Guid? ParentCategoryId) : IRequest<Result<Guid>>;