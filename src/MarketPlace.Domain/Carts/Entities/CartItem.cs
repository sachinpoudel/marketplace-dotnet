using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Carts.Entities;

public class CartItem: Entity<CartItemId>
{
  public Guid ProductId { get; private set; }
  public int Quantity { get; private set; }
  public CartId CartId { get; private set; }

  private CartItem() { } // For EF Core

  private CartItem(CartItemId itemId, Guid productId, int quantity) : base(itemId)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public static CartItem Create(Guid productId, int quantity)
    {
        return new CartItem(CartItemId.Create(), productId, quantity);
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
