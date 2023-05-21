using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Admin;
using Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

[Authorize("policy")]
public class ClientController : BaseController
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task<IActionResult> Add(AddClientRequest request, CancellationToken cancellationToken)
    {
        await _clientService.AddClient(request, cancellationToken);
        return Ok();
    }

    public async Task<IActionResult> Update(UpdateClientRequest request, CancellationToken cancellationToken)
    {
        await _clientService.UpdateClient(request, cancellationToken);
        return Ok();
    }

    public async Task<IActionResult> Delete(string clientId, CancellationToken cancellationToken)
    {
        await _clientService.DeleteClient(clientId, cancellationToken);
        return Ok();
    }
}