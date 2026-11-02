using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;

namespace MarketPlace.Domain.Orders.Entities;

public class OrderItem : Entity<OrderItemId>
{
  public ProductId ProductId { get; private set; }
  public OrderId OrderId { get; private set; }
  public VendorId VendorId { get; private set; }
  public int Quantity { get; private set; }
  public string ProductName { get; private set; } = default!;
  public decimal Price { get; private set; }
  public decimal SubTotal => Price * Quantity;

  private OrderItem() { } // For EF Core

  public void IncreaseQuantity(int quantity)
  {
      Quantity += quantity;
  }

  public void ChangeQuantity(int quantity)
  {
      Quantity = quantity;
  }
  public OrderItem(OrderItemId id,ProductId productId, int quantity, string productName, decimal price, OrderId orderId, VendorId vendorId) : base(id)
    {
       
        ProductId = productId;
        Quantity = quantity;
        ProductName = productName;
        Price = price;
        OrderId = orderId;
        VendorId = vendorId;
    }
   public static OrderItem Create(ProductId productId, int quantity, string productName, decimal price, OrderId orderId, VendorId vendorId)
    {
        var orderItemId = OrderItemId.Create();
        return new OrderItem(orderItemId, productId, quantity, productName, price, orderId, vendorId);
    }
}
