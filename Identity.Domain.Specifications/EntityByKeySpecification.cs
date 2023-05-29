using Dex.Entity;
using Dex.Specifications;

namespace Identity.Domain.Specifications;

/// <summary>
/// Спецификация для выборки сущностей по id
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
/// <typeparam name="TKey">Тип идентификатора у сущности</typeparam>
public class EntityByKeySpecification<TEntity, TKey> : Specification<TEntity>
    where TKey : IComparable
    where TEntity : IEntity<TKey>
{
    public EntityByKeySpecification(TKey key) : base(x => x.Id.Equals(key))
    {
    }
}