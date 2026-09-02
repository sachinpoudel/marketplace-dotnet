
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.Exceptions;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Enums;
using MarketPlace.Domain.Products.Events;

namespace MarketPlace.Domain.Products.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string Sku { get; private set; } = default!;
    public string ImageUrl { get; private set; } = default!;
    public string Tags { get; private set; } = default!;
    public ProductStatus Status { get; private set; }

    // Reference other aggregates by Id only — never by object navigation.
    // Need vendor name on a product listing? That's a query-side join, not this.
    public Guid VendorId { get; private set; }
    public Guid CategoryId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product(
        Guid id,
        string name,
        string description,
        decimal price,
        int stockQuantity,
        string sku,
        string imageUrl,
        string tags,
        Guid vendorId,
        Guid categoryId) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Sku = sku;
        ImageUrl = imageUrl;
        Tags = tags;
        Status = ProductStatus.Draft;
        VendorId = vendorId;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Product() { }

    public static Result<Product> Create(
        string name,
        string description,
        decimal price,
        int stockQuantity,
        string sku,
        string imageUrl,
        string tags,
        Guid vendorId,
        Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure(ProductError.ProductNameIsRequired());
        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("SKU is required.");

        var product = new Product(
            Guid.NewGuid(), name, description, price, stockQuantity,
            sku, imageUrl, tags, vendorId, categoryId);

        product.AddDomainEvent(new ProductCreatedEvent(product.Id, vendorId));
        return product;
    }

    public void UpdateDetails(string name, string description, decimal price, string tags)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        Name = name;
        Description = description;
        Price = price;
        Tags = tags;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    public void AdjustStock(int quantityDelta)
    {
        var newQuantity = StockQuantity + quantityDelta;
        if (newQuantity < 0)
            throw new DomainException("Stock quantity cannot go below zero.");

        StockQuantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;

        if (StockQuantity == 0)
            AddDomainEvent(new ProductOutOfStockEvent(Id));
    }

    public void Publish()
    {
        if (Status != ProductStatus.Draft)
            throw new DomainException("Only a draft product can be published.");

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delist()
    {
        Status = ProductStatus.Delisted;
        UpdatedAt = DateTime.UtcNow;
    }
}