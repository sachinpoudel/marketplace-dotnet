using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Products.Command.Update;




    public record UpdateProductCommand(Guid ProductId, string Name, string Description, decimal Price, int StockQuantity, IEnumerable<string> Tags) : IRequest<Result<UpdatedProductDto>>;