// using System.Threading;
// using System.Threading.Tasks;
// using Identity.Application.Abstractions.Services;
// using Identity.Models.Requests.Client;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Identity.Controllers.Admin;
//
// [Authorize("client")]
// public class ClientController : BaseController
// {
//     private readonly IClientService _clientService;
//
//     public ClientController(IClientService clientService)
//     {
//         _clientService = clientService;
//     }
//
//     public async Task<IActionResult> Get(int pageSize, int count)
//     {
//         
//     }
//
//     public async Task<IActionResult> Add(AddClientRequest request, CancellationToken cancellationToken)
//     {
//         await _clientService.AddClient(request, cancellationToken);
//         return Ok();
//     }
//
//     public async Task<IActionResult> Update(UpdateClientRequest request, CancellationToken cancellationToken)
//     {
//         await _clientService.UpdateClient(request, cancellationToken);
//         return Ok();
//     }
//
//     public async Task<IActionResult> Delete(string clientId, CancellationToken cancellationToken)
//     {
//         await _clientService.DeleteClient(clientId, cancellationToken);
//         return Ok();
//     }
// }