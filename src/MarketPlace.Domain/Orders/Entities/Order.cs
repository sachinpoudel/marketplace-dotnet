using System;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.Exceptions;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.Enums;
using MarketPlace.Domain.Orders.Events;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;

namespace MarketPlace.Domain.Orders.Entities;

public class Order : AggregateRoot<OrderId>
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime CancelledAt { get; private set; }
    public DateTime? ReservationExpiresAt { get; private set; }
    public OrderReservationStatus ReservationStatus { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public ShippingAddress ShippingAddress { get; private set; } = default!;
    private readonly List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public void CalculateSubTotal()
    {
        SubTotal = Items.Sum(item => item.SubTotal);
        TotalAmount = SubTotal + ShippingCost - DiscountAmount;
    }

    public Order(OrderId id, Guid userId, ShippingAddress shippingAddress, DateTime reservationExpiresAt)
        : base(id)
    {
        UserId = userId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ShippingAddress = shippingAddress;
        ReservationExpiresAt = reservationExpiresAt;
        ReservationStatus = Domain.Orders.Enums.OrderReservationStatus.Active;
        Status = OrderStatus.Pending;
    }

    public static Order Create(Guid userId, ShippingAddress shippingAddress)
    {
        var orderId = OrderId.Create();
        var reservationExpiration = DateTime.UtcNow.AddMinutes(15);
        return new Order(orderId, userId, shippingAddress,
            reservationExpiration);
    }

    public void AddItem(
        ProductId productId,
        int quantity,
        string productName,
        decimal price,
        VendorId vendorId
    )
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (price < 0)
            throw new DomainException("Price cannot be negative.");

        if (Status != OrderStatus.Pending)
            throw new DomainException("Items cannot be added to this order.");

        var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(productId, quantity, productName, price, Id, vendorId));
        }
        RecalculateTotals();
    }

    public void RemoveItem(ProductId productId)
    {
        EnsurePending();

        var item = Items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Item not found.");

        _items.Remove(item);

        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        SubTotal = Items.Sum(item => item.SubTotal);
        TotalAmount = SubTotal + ShippingCost - DiscountAmount;
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Order can only be modified while pending.");
    }

    public void ChangeItemQuantity(ProductId productId, int quantity)
    {
        EnsurePending();

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        var item = Items.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
            throw new DomainException("Item not found.");

        item.ChangeQuantity(quantity);

        RecalculateTotals();
    }

    public void SetShippingCost(decimal amount)
    {
        EnsurePending();

        if (amount < 0)
            throw new DomainException("Shipping cost cannot be negative.");

        ShippingCost = amount;

        RecalculateTotals();
    }

    public void ApplyDiscount(decimal amount)
    {
        EnsurePending();

        if (amount < 0)
            throw new DomainException("Discount cannot be negative.");

        if (amount > SubTotal)
            throw new DomainException("Discount cannot exceed subtotal.");

        DiscountAmount = amount;

        RecalculateTotals();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed.");

        if (Items.Count == 0)
            throw new DomainException("Cannot confirm an empty order.");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Order must be confirmed first.");

        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Processing)
            throw new DomainException("Order must be processing first.");

        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result Cancel()
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            throw new DomainException("This order cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        RaiseDomainEvent(new OrderCancelledEvent(Id, UserId));
        return Result.Success();
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Order must be shipped first.");

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result ConsumeReservation()
{
    if (ReservationStatus != OrderReservationStatus.Active)
        return Result.Failure(OrderError.InvalidReservation());

    ReservationStatus = OrderReservationStatus.Consumed;

    return Result.Success();
}

public Result ReleaseReservation()
{
    if (ReservationStatus != OrderReservationStatus.Active)
        return Result.Failure(OrderError.InvalidReservation());

    ReservationStatus = OrderReservationStatus.Released;

    return Result.Success();
}
}
