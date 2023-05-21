using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Admin;
using Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

[Authorize("policy")]
public class PolicyController : BaseController
{
    private readonly IPolicyService _policyService;

    public PolicyController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    public async Task<IActionResult> Add(AddPolicyRequest request, CancellationToken cancellationToken)
    {
        await _policyService.AddPolicy(request, cancellationToken);
        return Ok();
    }

    public async Task<IActionResult> Update(UpdatePolicyRequest request, CancellationToken cancellationToken)
    {
        await _policyService.UpdatePolicy(request, cancellationToken);
        return Ok();
    }

    public async Task<IActionResult> Delete(Guid policyId, CancellationToken cancellationToken)
    {
        await _policyService.DeletePolicy(policyId, cancellationToken);
        return Ok();
    }
}