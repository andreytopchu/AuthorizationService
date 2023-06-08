using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.Policy;
using Identity.Application.Abstractions.Models.Query.Policy;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

public class PolicyController : BaseController
{
    /// <summary>
    /// Создание полиси
    /// </summary>
    /// <param name="newPolicy">Новая полиси</param>
    /// <param name="addPolicyUseCase">UseCase добавления новой полиси</param>
    /// <param name="cancellation"></param>
    /// <returns>Созданная полиси</returns>
    /// <response code="200">Полиси успешно создана</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost]
    [ProducesResponseType(typeof(PolicyInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PolicyInfo>> AddPolicy([FromBody] AddPolicyRequest newPolicy, [FromServices] IUseCase<IAddPolicyCommand, PolicyInfo> addPolicyUseCase,
        CancellationToken cancellation)
    {
        if (newPolicy == null) throw new ArgumentNullException(nameof(newPolicy));
        if (addPolicyUseCase == null) throw new ArgumentNullException(nameof(addPolicyUseCase));

        return Ok(await addPolicyUseCase.Process(newPolicy, cancellation));
    }

    /// <summary>
    /// Изменение полиси
    /// </summary>
    /// <param name="editPolicy">Измененная полиси</param>
    /// <param name="updatePolicyUseCase">UseCase обновления полиси</param>
    /// <param name="cancellation"></param>
    /// <returns>Измененная полиси</returns>
    /// <response code="200">Полиси успешно изменена</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [ProducesResponseType(typeof(PolicyInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePolicy([FromBody] UpdatePolicyRequest editPolicy,
        [FromServices] IUseCase<IUpdatePolicyCommand, PolicyInfo> updatePolicyUseCase, CancellationToken cancellation)
    {
        if (updatePolicyUseCase == null) throw new ArgumentNullException(nameof(updatePolicyUseCase));

        return Ok(await updatePolicyUseCase.Process(editPolicy, cancellation));
    }

    /// <summary>
    /// Удаление полиси
    /// </summary>
    /// <param name="id">Id полиси</param>
    /// <param name="deletePolicyUseCase">UseCase удаления полиси</param>
    /// <param name="cancellation"></param>
    /// <returns>Статус-код</returns>
    /// <response code="200">Полиси успешно удалена</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePolicy([FromQuery] Guid id, [FromServices] IUseCase<IDeletePolicyCommand> deletePolicyUseCase,
        CancellationToken cancellation)
    {
        if (deletePolicyUseCase == null) throw new ArgumentNullException(nameof(deletePolicyUseCase));

        if (id == Guid.Empty) throw new ArgumentException("id is empty", nameof(id));

        await deletePolicyUseCase.Process(new DeletePolicyRequest
        {
            PolicyId = id
        }, cancellation);
        return Ok();
    }
}