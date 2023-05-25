using Identity.Application.Abstractions.Models.Command.Policy;

namespace Identity.Models.Requests.Policy;

public class AddPolicyRequest : BasePolicyRequest, IAddPolicyCommand
{
}