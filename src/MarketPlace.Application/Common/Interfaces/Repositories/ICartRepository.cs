using System;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Repositories;

public interface ICartRepository
{
  Task ClearCartAsync(CartId cartId, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
  Task<Cart> GetCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

  Task<Cart> AddItemToCartAsync(Cart cart, CancellationToken cancellationToken = default);
}
