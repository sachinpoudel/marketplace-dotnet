using System;
using System.Linq.Expressions;
using Hangfire;
using MarketPlace.Application.Common.Interfaces.BackgroundJobs;

namespace MarketPlace.Infrastructure.BackgroundJobs.Hangfire;

public class HangfireJobService : IBackgroundJobService
{
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }
}
