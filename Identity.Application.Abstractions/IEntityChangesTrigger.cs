using Identity.Application.Abstractions.Enum;

namespace Identity.Application.Abstractions;

public interface IEntityChangesTrigger
{
    Type EntityType { get; }
    void RunTrigger(IEnumerable<object> entities, EntityState state);
}