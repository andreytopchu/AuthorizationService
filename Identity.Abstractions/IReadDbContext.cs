// ReSharper properties

namespace Identity.Abstractions;

public interface IReadDbContext
{
    IQueryable<T> Get<T>() where T : class;
}