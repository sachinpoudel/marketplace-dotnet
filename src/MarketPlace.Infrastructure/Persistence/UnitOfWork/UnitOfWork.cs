using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;

namespace MarketPlace.Infrastructure.Persistence.UnitOfWork;

using System;
using System.Threading;
using System.Threading.Tasks;
using MarketPlace.Domain.Common.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _currentTransaction; // this means that we can have only one transaction at a time
    private bool _disposed;
    private readonly IPublisher _publisher;

    public IProductRepository ProductRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IVendorRepository VendorRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public ICartRepository CartRepository { get; }
    public IPaymentRepository PaymentRepository { get; }
    public IReviewRepository ReviewRepository { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IVendorRepository vendorRepository,
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IPaymentRepository paymentRepository,
        IReviewRepository reviewRepository,
        IPublisher publisher
    )
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        ProductRepository = productRepository;
        CategoryRepository = categoryRepository;
        VendorRepository = vendorRepository;
        OrderRepository = orderRepository;
        CartRepository = cartRepository;
        PaymentRepository = paymentRepository;
        ReviewRepository = reviewRepository;
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var domainEvents = _context
                .ChangeTracker.Entries<IAggregateRoot>()
                .Select(e => e.Entity)
                .SelectMany(e => e.DomainEvents)
                .ToList();
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            if (_currentTransaction != null) // if 
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            else // IF 
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            foreach (
                var aggregate in _context
                    .ChangeTracker.Entries<IAggregateRoot>()
                    .Select(e => e.Entity)
            )
            {
                aggregate.ClearDomainEvents();
            }
        }
        catch
        {
            if (_currentTransaction != null)
            {
                await RollbackAsync(cancellationToken);
            }
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
            return;

        try
        {
            await _currentTransaction.RollbackAsync(CancellationToken.None);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) // this method will be called before any operation that requires a transaction, like creating an order and payment
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");
        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _currentTransaction?.Dispose();
        _currentTransaction = null;
        _disposed = true;
    }
}
