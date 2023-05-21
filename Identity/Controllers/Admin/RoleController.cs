using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Admin;
using Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

[Authorize("role")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IActionResult> Add(AddRoleRequest request, CancellationToken cancellationToken)
    {
        await _roleService.AddRole(request, cancellationToken);
        return Ok();
    }

    public async Task<IActionResult> Update(UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        await _roleService.UpdateRole(request, cancellationToken);
        return Ok();
    }

    public async Task<IActionResult> Delete(Guid roleId, CancellationToken cancellationToken)
    {
        await _roleService.DeleteRole(roleId, cancellationToken);
        return Ok();
    }
}