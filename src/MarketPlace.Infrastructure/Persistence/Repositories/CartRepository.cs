using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public sealed class CartRepository(ApplicationDbContext context) : ICartRepository
{
    public async Task<Cart> AddItemToCartAsync(
        Cart cart,
        CancellationToken cancellationToken = default
    )
    {
        await context.Carts.AddAsync(cart, cancellationToken);
        return cart;
    }

    public async Task ClearCartAsync(CartId cartId, CancellationToken cancellationToken = default)
    {
        await context.Carts.Where(c => c.Id == cartId).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Cart?> GetCartByIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default
    )
    {
        var cartIdValue = CartId.Create(cartId);

        return await context
            .Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartIdValue, cancellationToken);
    }

    public async Task<Cart?> GetCartByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        return context.Carts.Update(cart).Entity;
    }
}
