// ReSharper properties

namespace Identity.Abstractions;

public interface IWriteDbContext : IReadDbContext
{
    object Add(object entity);
    object Update(object entity);
    object Remove(object entity);
}