using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Domain.Carts.Entities;

public class CartItem: Entity<CartItemId>
{
  public ProductId ProductId { get; private set; }
  public int Quantity { get; private set; }
  public CartId CartId { get; private set; }
  public double PriceAtTheTimeOfAdding { get; private set; }

  private CartItem() { } // For EF Core

  private CartItem(CartItemId itemId, ProductId productId, int quantity, double priceAtTheTimeOfAdding) : base(itemId)
    {
        ProductId = productId;
        Quantity = quantity;
        PriceAtTheTimeOfAdding = priceAtTheTimeOfAdding;
    }

    public static CartItem Create(ProductId productId, int quantity, double priceAtTheTimeOfAdding)
    {
        return new CartItem(CartItemId.Create(), productId, quantity, priceAtTheTimeOfAdding);
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
    public void Remove()
    {
    }
  public double GetTotalPrice()
  {
    return Quantity * PriceAtTheTimeOfAdding;
  }

}
