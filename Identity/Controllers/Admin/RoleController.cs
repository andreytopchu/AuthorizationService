using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.Role;
using Identity.Application.Abstractions.Models.Query.Role;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests;
using Identity.Models.Requests.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

public class RoleController : BaseController
{
    /// <summary>
    /// Получение роли по id
    /// </summary>
    /// <param name="id">Id роли</param>
    /// <param name="readRepository">Репозиторий ролей</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Информация о роли</returns>
    /// <response code="200">Информация о роли успешно возвращена</response>
    /// <response code="404">Роль не найдена</response>
    [HttpGet]
    [Authorize("role.read")]
    [ProducesResponseType(typeof(RoleInfo), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById([FromQuery] Guid id, [FromServices] IRoleReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        if (readRepository == null) throw new ArgumentNullException(nameof(readRepository));

        var result = await readRepository.GetByIdAsync<RoleInfo>(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Получение ролей
    /// </summary>
    /// <param name="request">Фильтры для пагинации</param>
    /// <param name="readRepository">Репозиторий ролей</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Список ролей</returns>
    /// <response code="200">Информация о ролях успешно возвращена</response>
    [HttpGet]
    [Authorize("role.read")]
    [ProducesResponseType(typeof(RoleInfo[]), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get([FromQuery] PaginationFilterRequest request, [FromServices] IRoleReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        if (readRepository == null) throw new ArgumentNullException(nameof(readRepository));

        var result = await readRepository.GetWithPaginationAsync<RoleInfo>(request, cancellationToken);
        return Ok(result);
    }

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
    [Authorize("role.write")]
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
    [Authorize("role.write")]
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
    [Authorize("role.write")]
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