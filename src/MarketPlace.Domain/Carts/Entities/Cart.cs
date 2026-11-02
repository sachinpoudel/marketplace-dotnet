using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Domain.Carts.Entities;

public class Cart : AggregateRoot<CartId>
{
    public Guid UserId { get; private set; }
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    private Cart(CartId id, Guid userId)
        : base(id)
    {
        UserId = userId;
    }

    public static Cart Create(Guid userId)
    {
        var cartId = CartId.Create();
        return new Cart(cartId, userId);
    }

    public void AddItem(ProductId productId, int quantity, decimal priceAtTheTimeOfAdding)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId.Equals(productId));
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            return;
        }

        // If no existing item is found, create a new one
        Items.Add(CartItem.Create(productId, quantity, priceAtTheTimeOfAdding)); // Assuming you have a way to get the price
    }

    public int CalculateTotalQuantity() => Items.Sum(item => item.Quantity);

    public decimal CalculateTotalPrice() =>
        Items.Sum(item => item.Quantity * item.PriceAtTheTimeOfAdding);

    public void RemoveItem(CartItemId cartItemId)
    {
        Items.Remove(Items.FirstOrDefault(i => i.Id.Equals(cartItemId)));
    }

    public void ChangeQuantity(ProductId productId, int newQuantity, int availableStock)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId.Equals(productId));
        if (existingItem == null)
        {
            throw new InvalidOperationException("Item not found in the cart.");
        }

        int oldQuantity = existingItem?.Quantity ?? 0;

        if (newQuantity > oldQuantity)
        {
            int amountToAdd = newQuantity - oldQuantity;
            if (amountToAdd > availableStock)
            {
                throw new InvalidOperationException(
                    $"Cannot increase quantity. Available stock is {availableStock}."
                );
            }
            existingItem?.UpdateQuantity(newQuantity);
        }
        else if (newQuantity < oldQuantity)
        {
            var amountToRemove = oldQuantity - newQuantity;
            if (newQuantity <= 0)
            {
                Items.Remove(existingItem);
                throw new InvalidOperationException("Quantity cannot be negative.");
            }
            existingItem?.UpdateQuantity(newQuantity);
        }
    }
}
