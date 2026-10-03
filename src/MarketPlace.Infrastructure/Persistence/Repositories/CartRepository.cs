using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public sealed class CartRepository(ApplicationDbContext context) : ICartRepository
{
    public async Task<Cart> AddItemToCartAsync(Cart cart, CancellationToken cancellationToken = default)
    {
           await context.Carts.AddAsync(cart, cancellationToken);
           return cart;
    }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Cart> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
    