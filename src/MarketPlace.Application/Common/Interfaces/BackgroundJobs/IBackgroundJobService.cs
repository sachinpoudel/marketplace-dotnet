using System;
using System.Linq.Expressions;

namespace MarketPlace.Application.Common.Interfaces.BackgroundJobs;

public interface IBackgroundJobService
{
 string Enqueue<T>(Expression<Func<T ,Task>> methodCall);
}
        