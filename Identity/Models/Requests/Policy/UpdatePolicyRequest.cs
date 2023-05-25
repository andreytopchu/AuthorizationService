using System;
using Identity.Application.Abstractions.Models.Command.Policy;

namespace Identity.Models.Requests.Policy;

public class UpdatePolicyRequest : BasePolicyRequest, IUpdatePolicyCommand
{
    public Guid Id { get; init; }
}