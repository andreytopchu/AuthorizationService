namespace Identity.Domain.Exceptions;

public sealed class PolicyNotFoundException : NotFoundException
{
    public PolicyNotFoundException(Guid roleId)
    {
        Data[nameof(roleId)] = roleId;
    }
}