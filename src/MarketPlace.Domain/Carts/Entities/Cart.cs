using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Domain.Carts.Entities;

public class Cart : AggregateRoot<CartId>
{
    public Guid UserId { get; private set; }
    public ICollection<CartItem> Items => _items.AsReadOnly();
    private readonly List<CartItem> _items = new();

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
        var existingItem = _items.FirstOrDefault(i => i.ProductId.Equals(productId));
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            return;
        }

        // If no existing item is found, create a new one
        _items.Add(CartItem.Create(productId, quantity, priceAtTheTimeOfAdding)); // Assuming you have a way to get the price
    }

    public int CalculateTotalQuantity() => _items.Sum(item => item.Quantity);

    public decimal CalculateTotalPrice() =>
        _items.Sum(item => item.Quantity * item.PriceAtTheTimeOfAdding);

    public void RemoveItem(CartItemId cartItemId)
    {
        _items.Remove(_items.FirstOrDefault(i => i.Id.Equals(cartItemId)));
    }
    public void ClearCart()
    {
        _items.Clear();
    }
    public void ChangeQuantity(ProductId productId, int newQuantity, int availableStock)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId.Equals(productId));
        if (existingItem == null)
        {
            throw new InvalidOperationException("Item not found in the cart.");
        }

        int oldQuantity = existingItem?.Quantity ?? 0;

        if (newQuantity > oldQuantity) // new = 5 and old = 3, then we need to add 2 more, check if stock is available
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
        else if (newQuantity < oldQuantity) // new = 2 and old = 5, then we need to remove 3, no need to check stock
        {
            var amountToRemove = oldQuantity - newQuantity;
            if (newQuantity <= 0)
            {
                _items.Remove(existingItem); // Remove the item if the new quantity is zero or less
            }
            else
            {
                throw new InvalidOperationException("Quantity cannot be negative.");
            }
            existingItem?.UpdateQuantity(newQuantity);
        }
    }
}
