using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.Repositories.Client;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests;
using Identity.Models.Requests.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

public class ClientController : BaseController
{
    /// <summary>
    /// Получение клиента по id
    /// </summary>
    /// <param name="clientId">Id клиента</param>
    /// <param name="readRepository">Репозиторий клиентов</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Информация о клиенте</returns>
    /// <response code="200">Информация о клиенте успешно возвращена</response>
    /// <response code="404">Клиент не найден</response>
    [HttpGet]
    [Authorize("client.read")]
    [ProducesResponseType(typeof(ClientInfo), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById([FromQuery] string clientId, [FromServices] IClientReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        if (readRepository == null) throw new ArgumentNullException(nameof(readRepository));

        var result = await readRepository.GetById(clientId, cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Получение клиентов
    /// </summary>
    /// <param name="request">Фильтры для пагинации</param>
    /// <param name="readRepository">Репозиторий клиентов</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Список клиентов</returns>
    /// <response code="200">Информация о клиентах успешно возвращена</response>
    [HttpGet]
    [Authorize("client.read")]
    [ProducesResponseType(typeof(ClientInfo[]), StatusCodes.Status200OK)]
    public async Task<ActionResult> Get([FromQuery] PaginationFilterRequest request, [FromServices] IClientReadRepository readRepository,
        CancellationToken cancellationToken)
    {
        if (readRepository == null) throw new ArgumentNullException(nameof(readRepository));

        var result = await readRepository.Get(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Создание клиента
    /// </summary>
    /// <param name="newClient">Новый клиент</param>
    /// <param name="addClientUseCase">UseCase добавления нового клиента</param>
    /// <param name="cancellation"></param>
    /// <returns>Созданный клиент</returns>
    /// <response code="200">Клиент успешно создан</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost]
    [Authorize("client.write")]
    [ProducesResponseType(typeof(ClientInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientInfo>> AddClient([FromBody] AddClientRequest newClient,
        [FromServices] IUseCase<IAddClientCommand, ClientInfo> addClientUseCase,
        CancellationToken cancellation)
    {
        if (newClient == null) throw new ArgumentNullException(nameof(newClient));
        if (addClientUseCase == null) throw new ArgumentNullException(nameof(addClientUseCase));

        return Ok(await addClientUseCase.Process(newClient, cancellation));
    }

    /// <summary>
    /// Изменение клиента
    /// </summary>
    /// <param name="editClient">Измененный клиент</param>
    /// <param name="updateClientUseCase">UseCase обновления клиента</param>
    /// <param name="cancellation"></param>
    /// <returns>Измененный клиент</returns>
    /// <response code="200">Клиент успешно изменен</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [Authorize("client.write")]
    [ProducesResponseType(typeof(ClientInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateClient([FromBody] UpdateClientRequest editClient,
        [FromServices] IUseCase<IUpdateClientCommand, ClientInfo> updateClientUseCase, CancellationToken cancellation)
    {
        if (updateClientUseCase == null) throw new ArgumentNullException(nameof(updateClientUseCase));

        return Ok(await updateClientUseCase.Process(editClient, cancellation));
    }

    /// <summary>
    /// Удаление клиента
    /// </summary>
    /// <param name="clientId">Id клиента</param>
    /// <param name="deleteClientUseCase">UseCase удаления клиента</param>
    /// <param name="cancellation"></param>
    /// <returns>Статус-код</returns>
    /// <response code="200">Клиент успешно удалена</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete]
    [Authorize("client.write")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteClient([FromQuery] string clientId, [FromServices] IUseCase<IDeleteClientCommand> deleteClientUseCase,
        CancellationToken cancellation)
    {
        if (deleteClientUseCase == null) throw new ArgumentNullException(nameof(deleteClientUseCase));

        await deleteClientUseCase.Process(new DeleteClientRequest
        {
            ClientId = clientId
        }, cancellation);
        return Ok();
    }
}