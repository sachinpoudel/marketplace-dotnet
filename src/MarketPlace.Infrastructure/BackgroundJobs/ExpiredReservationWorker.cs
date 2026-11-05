using System;
using MarketPlace.Application.Features.Reservations.ExpireReservations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Infrastructure.BackgroundJobs;

public sealed class ExpiredReservationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredReservationWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        // Putting the wait at the top ensures a strict 1-minute cadence right from startup
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                logger.LogInformation("Processing expired reservations...");

                await using var scope = scopeFactory.CreateAsyncScope(); // this ensures that the scope is disposed of properly after each iteration, preventing memory leaks and ensuring that scoped services are released correctly.
                var sender = scope.ServiceProvider.GetRequiredService<ISender>(); // this retrieves an instance of ISender from the scoped service provider, allowing the worker to send MediatR commands within the scope of the current iteration.

                // The next tick will not wait or execute until this operation completely finishes
                await sender.Send(new ExpireReservationsCommand(), stoppingToken); // this sends the ExpireReservationsCommand using MediatR, triggering the logic to process and expire reservations that have passed their expiration time.
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Clean exit when app shuts down
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process expired reservations.");
            }
        }
    }
}
