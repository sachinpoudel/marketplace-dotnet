using MarketPlace.Application.Common.Interfaces.Repositories;

namespace MaketPlace.Application.Common.Interfaces.UnitOfWork;


public interface IUnitOfWork
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IVendorRepository VendorRepository { get; }
    IOrderRepository OrderRepository { get; }
    ICartRepository CartRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IReviewRepository ReviewRepository { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
}