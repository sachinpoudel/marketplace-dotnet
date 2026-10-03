using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Products.Command.Delete;




public class DeleteProductCommandHandle(IUnitOfWork unitOfWork, IProductRepository productRepository) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        if(product == null)
        {
            return Result.Failure(ProductError.ProductNotFound());
        }
      
        product.Delete();

        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}