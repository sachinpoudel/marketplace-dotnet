
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.Exceptions;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Common.ValueObjects;
using MarketPlace.Domain.Products.Enums;
using MarketPlace.Domain.Products.Events;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;
using static System.Net.Mime.MediaTypeNames;

namespace MarketPlace.Domain.Products.Entities;

public sealed class Product : AggregateRoot<ProductId>
{


    private  List<CategoryId> _categoryIds = new();
    private List<Tag> _tags = new();

    private List<Img> _images = new();

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string Sku { get; private set; } = default!;
    public ProductStatus Status { get; private set; }

    public IReadOnlyCollection<Img> Images => _images.AsReadOnly();
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<CategoryId> CategoryIds => _categoryIds.AsReadOnly();



    public VendorId VendorId { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product(
        ProductId id,
        string name,
        string description,
        decimal price,
        int stockQuantity,
        string sku,
        IEnumerable<Img> images,
        IEnumerable<Tag> tags,
        ProductStatus status,
      VendorId vendorId
        ) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Sku = sku;
        _images = new List<Img>(images.Select(img => new Img(img.Url)));
        _tags = new List<Tag>(tags.Select(tag => Tag.Create(tag.Name)));
        Status = status;
        VendorId = vendorId;
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
        IEnumerable<Img> images,
        IEnumerable<Tag> tags,
      
        VendorId vendorId,
    IEnumerable<CategoryId> categoryIds
        )
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
            ProductId.Create(), name, description, price, stockQuantity,
             sku, images, tags, ProductStatus.Active, vendorId
            );
        foreach (var categoryId in categoryIds)
        {
            product.AddCategory(categoryId);
        }
        product.AddDomainEvent(new ProductCreatedEvent(product.Id, vendorId));
        return product;
    }
    public void AddCategory(CategoryId categoryId)
    {
        if (!_categoryIds.Contains(categoryId))
            _categoryIds.Add(categoryId);
    }
    public void UpdateDetails(string name, string description, decimal price, IEnumerable<string> tags)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        if (price < 0)
            throw new DomainException("Product price cannot be negative.");

        Name = name;
        Description = description;
        Price = price;
        _tags = new List<Tag>(tags.Select(tag => Tag.Create(tag)));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImages(IEnumerable<Img> images)
    {
        _images = new List<Img>(images.Select(img => new Img(img.Url)));
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
