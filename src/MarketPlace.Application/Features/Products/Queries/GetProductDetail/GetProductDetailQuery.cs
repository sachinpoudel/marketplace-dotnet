using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Products.Queries.GetProductById;

public record GetProductDetailQuery(Guid ProductId) : IRequest<Result<ProductDetailDto>>;


