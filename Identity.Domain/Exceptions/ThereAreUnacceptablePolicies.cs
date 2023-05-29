namespace Identity.Domain.Exceptions;

public sealed class ThereAreUnacceptablePolicies : PreconditionFailedException
{
    /// <summary>
    /// В коллекции есть недопустимые права доступа
    /// </summary>
    /// <remarks>Status:412</remarks>
    public ThereAreUnacceptablePolicies(IEnumerable<Guid> policyIds) : base("There are unacceptable policies")
    {
        Data["PolicyIds"] = policyIds;
    }
}