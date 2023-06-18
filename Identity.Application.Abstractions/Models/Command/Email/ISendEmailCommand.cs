namespace Identity.Application.Abstractions.Models.Command.Email
{
    /// <summary>
    /// Команда шины представляющая собой текст 'E-Mail' сообщения и E-Mail адреса получателей.
    /// </summary>
    public interface ISendEmailCommand
    {
        /// <summary>
        /// Уникальный идентификатор нотификации из БД админки.
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// Текст темы 'E-Mail' сообщения.
        /// </summary>
        string Heading { get; }

        /// <summary>
        /// Текст 'E-Mail' сообщения.
        /// </summary>
        string MessageText { get; }

        IReadOnlyCollection<string> Emails { get; }
    }
}