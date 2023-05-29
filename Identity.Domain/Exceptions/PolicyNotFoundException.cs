namespace Identity.Domain.Exceptions;

public class PolicyNotFoundException : NotFoundException
{
    public PolicyNotFoundException(Guid roleId)
    {
        Data[nameof(roleId)] = roleId;
    }
}