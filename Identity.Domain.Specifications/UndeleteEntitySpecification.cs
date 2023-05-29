using Dex.Entity;
using Dex.Specifications;

namespace Identity.Domain.Specifications;

public class UndeleteEntitySpecification<TEntity> : Specification<TEntity> where TEntity : IDeletable
{
    public UndeleteEntitySpecification() : base(db => !db.DeletedUtc.HasValue)
    {
    }
}