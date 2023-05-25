// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Identity.Application.Abstractions.Services;
// using Identity.Models.Requests.User;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Identity.Controllers.Admin;
//
// [Authorize("user")]
// public class UserController : BaseController
// {
//     private readonly IUserService _userService;
//
//     public UserController(IUserService userService)
//     {
//         _userService = userService;
//     }
//
//     public async Task<IActionResult> Add(AddUserRequest request, CancellationToken cancellationToken)
//     {
//         await _userService.AddUser(request, cancellationToken);
//         return Ok();
//     }
//
//     public async Task<IActionResult> Update(UpdateUserRequest request, CancellationToken cancellationToken)
//     {
//         await _userService.UpdateUser(request, cancellationToken);
//         return Ok();
//     }
//
//     public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
//     {
//         await _userService.DeleteUser(userId, cancellationToken);
//         return Ok();
//     }
// }