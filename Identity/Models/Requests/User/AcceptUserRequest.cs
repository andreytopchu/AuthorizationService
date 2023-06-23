using System.ComponentModel.DataAnnotations;
using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User
{
    /// <summary>
    /// Запрос на подтверждение пароля пользователя
    /// </summary>
    public record AcceptUserRequest : IAcceptUserCommand
    {
        /// <summary>
        /// Пароль для аккаунта
        /// </summary>
        public string Password { get; init; }

        /// <summary>
        /// Токен, получаемый из URL при открытии письма, либо в БД
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Token { get; init; }

        public EmailType EmailType { get; set; }
    }
}