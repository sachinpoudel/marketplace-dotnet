using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Products.Command.Create;


public record CreateProductCommand(string Name, string Description, decimal Price,
int StockQuantity,
string Sku,
string? ImageUrl,
string Tags,

 IReadOnlyList<Guid> CategoryIds, Guid VendorId) : IRequest<Result<ProductsListItemDto>>;       