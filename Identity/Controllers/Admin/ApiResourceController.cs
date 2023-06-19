using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.Repositories.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests.ApiResource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

public class ApiResourceController : BaseController
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResourceInfo[]), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById([FromServices] IApiResourceReadRepository readRepository, int id,
        CancellationToken cancellationToken)
    {
        if (readRepository == null) throw new ArgumentNullException(nameof(readRepository));

        var result = await readRepository.GetById(id, cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Создание ресурса
    /// </summary>
    /// <param name="newResource">Новый ресурс</param>
    /// <param name="addResourceUseCase">UseCase добавления нового ресурса</param>
    /// <param name="cancellation"></param>
    /// <returns>Созданный ресурс</returns>
    /// <response code="200">Ресурс успешно создан</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResourceInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResourceInfo>> AddResource([FromBody] AddApiResourceRequest newResource,
        [FromServices] IUseCase<IAddApiResourceCommand, ApiResourceInfo> addResourceUseCase,
        CancellationToken cancellation)
    {
        if (newResource == null) throw new ArgumentNullException(nameof(newResource));
        if (addResourceUseCase == null) throw new ArgumentNullException(nameof(addResourceUseCase));

        return Ok(await addResourceUseCase.Process(newResource, cancellation));
    }

    /// <summary>
    /// Изменение ресурсов
    /// </summary>
    /// <param name="editResource">Измененный ресурс</param>
    /// <param name="updateResourceUseCase">UseCase обновления ресурсов</param>
    /// <param name="cancellation"></param>
    /// <returns>Измененный ресурс</returns>
    /// <response code="200">Ресурс успешно изменен</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResourceInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateResource([FromBody] UpdateApiResourceRequest editResource,
        [FromServices] IUseCase<IUpdateApiResourceCommand, ApiResourceInfo> updateResourceUseCase, CancellationToken cancellation)
    {
        if (updateResourceUseCase == null) throw new ArgumentNullException(nameof(updateResourceUseCase));

        return Ok(await updateResourceUseCase.Process(editResource, cancellation));
    }

    /// <summary>
    /// Удаление ресурсов
    /// </summary>
    /// <param name="resourceId">Id ресурсов</param>
    /// <param name="deleteResourceUseCase">UseCase удаления ресурсов</param>
    /// <param name="cancellation"></param>
    /// <returns>Статус-код</returns>
    /// <response code="200">Ресурс успешно удалена</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteResource([FromQuery] int resourceId, [FromServices] IUseCase<IDeleteApiResourceCommand> deleteResourceUseCase,
        CancellationToken cancellation)
    {
        if (deleteResourceUseCase == null) throw new ArgumentNullException(nameof(deleteResourceUseCase));

        await deleteResourceUseCase.Process(new DeleteApiResourceRequest
        {
            ApiResourceId = resourceId
        }, cancellation);
        return Ok();
    }
}