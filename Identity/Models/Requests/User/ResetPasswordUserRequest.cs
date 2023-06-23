using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User
{
    /// <summary>
    /// Восстановление пароля сотрудника
    /// </summary>
    public record ResetPasswordUserRequest : IResetPasswordUserCommand
    {
        /// <summary>
        /// Адрес эл.почты, на который высылается письмо с восстановлением пароля
        /// </summary>
        public string Email { get; init; }
    }
}