using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.UseCases;
using Identity.ExceptionFilter;
using Identity.Models.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers.User
{
    public class UserActivationController : BaseController
    {
        /// <summary>
        /// Подтверждение пароля пользователя при регистрации
        /// </summary>
        /// <param name="acceptUser">Запрос на подтверждение пароля пользователя</param>
        /// <param name="acceptUserUseCase">UseCase подтверждения пароля пользователя при регистрации</param>
        /// <param name="cancellation"></param>
        /// <response code="200">Регистрация пользователя произведена успешно</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="401">Отсутствует авторизация в систему</response>
        /// <response code="404">Не найдено</response>
        /// <response code="403">Нет прав на совершение действия</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptRegisterByToken([FromBody] AcceptUserRequest acceptUser,
            [FromServices] IUseCase<IAcceptUserCommand> acceptUserUseCase, CancellationToken cancellation)
        {
            if (acceptUser == null) throw new ArgumentNullException(nameof(acceptUser));
            if (acceptUserUseCase == null) throw new ArgumentNullException(nameof(acceptUserUseCase));

            await acceptUserUseCase.Process(acceptUser, cancellation);

            return Ok();
        }

        /// <summary>
        /// Подтверждение пароля пользователя при восст.пароля
        /// </summary>
        /// <param name="restorePasswordRequest">Запрос на подтверждение пароля пользователя</param>
        /// <param name="recoveryPasswordUseCase">UseCase подтверждения пароля пользователя при регистрации</param>
        /// <param name="cancellation"></param>
        /// <response code="200">Восст.пароля пользователя произведено успешно</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="401">Отсутствует авторизация в систему</response>
        /// <response code="404">Не найдено</response>
        /// <response code="403">Нет прав на совершение действия</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> RestorePasswordByToken([FromBody] UserRestorePasswordRequest restorePasswordRequest,
            [FromServices] IUseCase<IUserRestorePasswordCommand> recoveryPasswordUseCase, CancellationToken cancellation)
        {
            if (restorePasswordRequest == null) throw new ArgumentNullException(nameof(restorePasswordRequest));
            if (recoveryPasswordUseCase == null) throw new ArgumentNullException(nameof(recoveryPasswordUseCase));

            await recoveryPasswordUseCase.Process(restorePasswordRequest, cancellation);

            return Ok();
        }

        /// <summary>
        /// Функция "Забыли пароль?"
        /// </summary>
        /// <param name="email">Адрес эл.почты, на который высылается письмо с восстановлением пароля</param>
        /// <param name="resetPasswordUserUseCase">UseCase восстановления пароля пользователя</param>
        /// <param name="cancellation"></param>
        /// <response code="200">Письмо для восст.пароля отправлено успешно</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="401">Отсутствует авторизация в систему</response>
        /// <response code="404">Не найдено</response>
        /// <response code="403">Нет прав на совершение действия</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> Forgot([FromBody] string email, [FromServices] IUseCase<IResetPasswordUserCommand> resetPasswordUserUseCase,
            CancellationToken cancellation)
        {
            if (resetPasswordUserUseCase == null) throw new ArgumentNullException(nameof(resetPasswordUserUseCase));
            if (email == null) throw new ArgumentNullException(nameof(email));

            await resetPasswordUserUseCase.Process(new ResetPasswordUserRequest
            {
                Email = email,
            }, cancellation);

            return Ok();
        }
    }
}