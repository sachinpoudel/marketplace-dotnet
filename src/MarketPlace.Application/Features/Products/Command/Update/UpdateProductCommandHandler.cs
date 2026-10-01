using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Products.Command.Update;



public class UpdateProductCommandHandler(ICurrentUser currentUser,IProductRepository productRepository, IAuthService authService, IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result<UpdatedProductDto>>
{
    public async Task<Result<UpdatedProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.UserId;

        if (currentUserId == null)
        {
            
            return Result<UpdatedProductDto>.Failure(UserError.UserNotAuthenticated());
        }
        var userRole = await authService.IsInRoleAsync(currentUserId, "Vendor", cancellationToken);


        if (!userRole)
        {
            return Result<UpdatedProductDto>.Failure(UserError.UserNotAuthorized());
        }
var productId = ProductId.Create(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        if (product == null)
        {
            return Result<UpdatedProductDto>.Failure(ProductError.ProductNotFound());
        }

        product.UpdateDetails(request.Name, request.Description, request.Price, request.StockQuantity, request.Tags);
        await productRepository.Update(product);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result<UpdatedProductDto>.Success(new UpdatedProductDto(product.Id.Value, product.Name, product.Description, product.Price, product.StockQuantity, product.Tags.Select(t => t.Name).ToList()));
    }
}