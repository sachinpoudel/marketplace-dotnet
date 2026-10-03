using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Domain.Carts.Entities;

public class Cart : AggregateRoot<CartId>
{
public Guid UserId { get; private set; }
public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

private Cart(CartId id, Guid userId) : base(id)
    {
        UserId = userId;
    }



    public static Cart Create(Guid userId)
    {
        var cartId = CartId.Create();
        return new Cart(cartId, userId);
    }

    public void AddItem(ProductId productId, int quantity)
    {
    var existingItem = Items.FirstOrDefault(i => i.ProductId == productId.Value);

    if(existingItem != null)
    {
        existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        return;
    }

    
        Items.Add(CartItem.Create(productId.Value, quantity));
    }
}
