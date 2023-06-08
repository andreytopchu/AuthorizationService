using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

public class RoleController : BaseController
{
    /// <summary>
    /// Создание роли
    /// </summary>
    /// <param name="newRole">Новая роль</param>
    /// <param name="addRoleUseCase">UseCase добавления новой роли</param>
    /// <param name="cancellation"></param>
    /// <returns>Созданная роль</returns>
    /// <response code="200">Роль успешно создана</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoleInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RoleInfo>> AddRole([FromBody] AddRoleRequest newRole, [FromServices] IUseCase<IAddRoleCommand, RoleInfo> addRoleUseCase,
        CancellationToken cancellation)
    {
        if (newRole == null) throw new ArgumentNullException(nameof(newRole));
        if (addRoleUseCase == null) throw new ArgumentNullException(nameof(addRoleUseCase));

        return Ok(await addRoleUseCase.Process(newRole, cancellation));
    }

    /// <summary>
    /// Изменение роли
    /// </summary>
    /// <param name="editRole">Измененная роль</param>
    /// <param name="updateRoleUseCase">UseCase обновления роли</param>
    /// <param name="cancellation"></param>
    /// <returns>Измененная роль</returns>
    /// <response code="200">Роль успешно изменена</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [ProducesResponseType(typeof(RoleInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest editRole,
        [FromServices] IUseCase<IUpdateRoleCommand, RoleInfo> updateRoleUseCase, CancellationToken cancellation)
    {
        if (updateRoleUseCase == null) throw new ArgumentNullException(nameof(updateRoleUseCase));

        return Ok(await updateRoleUseCase.Process(editRole, cancellation));
    }

    /// <summary>
    /// Удаление роли
    /// </summary>
    /// <param name="id">Id роли</param>
    /// <param name="deleteRoleUseCase">UseCase удаления роли</param>
    /// <param name="cancellation"></param>
    /// <returns>Статус-код</returns>
    /// <response code="200">Роль успешно удалена</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRole([FromQuery] Guid id, [FromServices] IUseCase<IDeleteRoleCommand> deleteRoleUseCase,
        CancellationToken cancellation)
    {
        if (deleteRoleUseCase == null) throw new ArgumentNullException(nameof(deleteRoleUseCase));

        if (id == Guid.Empty) throw new ArgumentException("id is empty", nameof(id));

        await deleteRoleUseCase.Process(new DeleteRoleRequest
        {
            RoleId = id
        }, cancellation);
        return Ok();
    }
}