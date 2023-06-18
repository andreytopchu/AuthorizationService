using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.Admin;

public class UserController : BaseController
{
    /// <summary>
    /// Создание пользователя
    /// </summary>
    /// <param name="newUser">Новый пользователь</param>
    /// <param name="addUserUseCase">UseCase добавления нового пользователя</param>
    /// <param name="cancellation"></param>
    /// <returns>Созданный пользователь</returns>
    /// <response code="200">Пользователь успешно создан</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserInfo>> AddUser([FromBody] AddUserRequest newUser,
        [FromServices] IUseCase<IAddUserCommand, UserInfo> addUserUseCase,
        CancellationToken cancellation)
    {
        if (newUser == null) throw new ArgumentNullException(nameof(newUser));
        if (addUserUseCase == null) throw new ArgumentNullException(nameof(addUserUseCase));

        return Ok(await addUserUseCase.Process(newUser, cancellation));
    }

    /// <summary>
    /// Изменение пользователя
    /// </summary>
    /// <param name="editUser">Измененный пользователь</param>
    /// <param name="updateUserUseCase">UseCase обновления пользователя</param>
    /// <param name="cancellation"></param>
    /// <returns>Измененный пользователь</returns>
    /// <response code="200">Пользователь успешно изменен</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest editUser,
        [FromServices] IUseCase<IUpdateUserCommand, UserInfo> updateUserUseCase, CancellationToken cancellation)
    {
        if (updateUserUseCase == null) throw new ArgumentNullException(nameof(updateUserUseCase));

        return Ok(await updateUserUseCase.Process(editUser, cancellation));
    }

    /// <summary>
    /// Генерация нового приглашения с новым токеном активации
    /// </summary>
    /// <param name="userId">Id пользователя в БД</param>
    /// <param name="updateUserInvitationUseCase">UseCase генерации нового приглашения с новым токеном активации</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Информация о пользователе</returns>
    /// <response code="200">Приглашение успешно сгенерировано</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="404">Запись не найдена</response>
    /// <response code="409">Конфликт</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpPut]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserInfo>> UpdateInvitation([FromBody] Guid userId,
        [FromServices] IUseCase<IUpdateUserInvitationCommand, UserInfo> updateUserInvitationUseCase, CancellationToken cancellationToken)
    {
        if (updateUserInvitationUseCase == null) throw new ArgumentNullException(nameof(updateUserInvitationUseCase));
        if (userId == Guid.Empty) throw new ArgumentException(null, nameof(userId));

        return Ok(await updateUserInvitationUseCase.Process(new UpdateUserInvitationRequest
        {
            UserId = userId
        }, cancellationToken));
    }

    /// <summary>
    /// Удаление пользователя
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <param name="deleteUserUseCase">UseCase удаления пользователя</param>
    /// <param name="cancellation"></param>
    /// <returns>Статус-код</returns>
    /// <response code="200">Пользователь успешно удален</response>
    /// <response code="400">Некорректный запрос</response>
    /// <response code="401">Отсутствует авторизация в систему</response>
    /// <response code="403">Нет прав на совершение действия</response>
    /// <response code="500">Ошибка сервера</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid id, [FromServices] IUseCase<IDeleteUserCommand> deleteUserUseCase,
        CancellationToken cancellation)
    {
        if (deleteUserUseCase == null) throw new ArgumentNullException(nameof(deleteUserUseCase));

        if (id == Guid.Empty) throw new ArgumentException("id is empty", nameof(id));

        await deleteUserUseCase.Process(new DeleteUserRequest
        {
            UserId = id
        }, cancellation);
        return Ok();
    }
}