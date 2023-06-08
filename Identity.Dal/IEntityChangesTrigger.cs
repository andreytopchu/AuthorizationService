using System;
using System.Collections.Generic;
using Identity.Dal.Enum;

namespace Identity.Dal;

public interface IEntityChangesTrigger
{
    Type EntityType { get; }
    void RunTrigger(IEnumerable<object> entities, EntityState state);
}