using System;
using Identity.Application.Abstractions.Models.Command.Policy;

namespace Identity.Models.Requests.Policy;

public class DeletePolicyRequest : IDeletePolicyCommand
{
    public Guid PolicyId { get; init; }
}