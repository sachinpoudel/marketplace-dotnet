using System;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Products.Command.Delete;

public record DeleteProductCommand
(
    Guid ProductId
) : IRequest<Result>;
