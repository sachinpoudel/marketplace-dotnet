namespace MarketPlace.Application.Features.Orders.Dtos;

public record class OrderDetailDto
(
            Guid Id,
            Guid UserId,
            ShippingAddressDto ShippingAddress,
            string Status,
            List<OrderItemDto> Items


            );

public record class ShippingAddressDto(
    string FullName,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PhoneNumber
);