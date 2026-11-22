using System;

namespace MarketPlace.Infrastructure.BackgroundJobs.Hangfire;

public class HangfireSettings
{
    public const string SectionName = "HangfireSettings";
  public int WorkerCount { get; set; } = 1;
}
