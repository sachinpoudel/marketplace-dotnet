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
    private IDbContextTransaction _currentTransaction;
    private bool _disposed;
    private readonly IPublisher _publisher;

    public IProductRepository ProductRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IVendorRepository VendorRepository { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IVendorRepository vendorRepository,
        IPublisher publisher)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        ProductRepository = productRepository;
        CategoryRepository = categoryRepository;
        VendorRepository = vendorRepository;
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
      {
          try
          {
              var domainEvents = _context.ChangeTracker
                  .Entries<IAggregateRoot>()
                  .Select(e => e.Entity)
                  .SelectMany(e => e.DomainEvents)
                  .ToList();
  
              foreach (var aggregate in _context.ChangeTracker.Entries<IAggregateRoot>().Select(e => e.Entity))
              {
                  aggregate.ClearDomainEvents();
              }
  
              if (_currentTransaction != null)
              {
                  await _context.SaveChangesAsync(cancellationToken);
                  await _currentTransaction.CommitAsync(cancellationToken);
              }
              else
              {
                  await _context.SaveChangesAsync(cancellationToken);
              }
  
              foreach (var domainEvent in domainEvents)
              {
                  await _publisher.Publish(domainEvent, cancellationToken);
              }
          }
          catch
          {
              await RollbackAsync(cancellationToken);
              throw;
          }
          finally
          {
              await DisposeTransactionAsync();
          }
      }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
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
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
                _currentTransaction?.Dispose();
            }
            _disposed = true;
        }
    }






    
}
