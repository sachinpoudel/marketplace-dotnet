using MarketPlace.Application.Common.Interfaces.Repositories;

namespace MaketPlace.Application.Common.Interfaces.UnitOfWork;


public interface IUnitOfWork
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IVendorRepository VendorRepository { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}