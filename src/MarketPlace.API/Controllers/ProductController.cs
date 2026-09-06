using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Products.Command.Create;
using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Application.Features.Products.Queries.GetProductsList;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductController(IMediator mediator) : ControllerBase
{


    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new CreateProductCommand(

            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,


            request.Sku,
            request.ImageUrl,
            request.Tags,
            request.CategoryIds,
            request.VendorId);

        var result = await mediator.Send(product, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] GetProductsListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
