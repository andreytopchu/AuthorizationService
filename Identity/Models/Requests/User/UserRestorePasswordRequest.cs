// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User
{
    /// <summary>
    /// Запрос на подтверждение пароля пользователя при восстановлении
    /// </summary>
    public record UserRestorePasswordRequest : IUserRestorePasswordCommand
    {
        /// <summary>
        /// Пароль для аккаунта (8-20 символов, >=1 цифра, >=1 Z-z)
        /// </summary>
        [ValidatePassword]
        public string Password { get; init; }

        /// <summary>
        /// Токен, получаемый из URL при открытии письма, либо в БД
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Token { get; init; }

        public EmailType EmailType { get; set; }
    }
}