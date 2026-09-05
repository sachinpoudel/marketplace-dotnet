using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Common.Models;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize; // clamp — never let a client request 10,000 rows

        // COUNT(*) as one SQL query — does NOT load rows into memory
        var count = await source.CountAsync(cancellationToken);

        // OFFSET/FETCH pushed into SQL — only this page's rows are ever materialized
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}