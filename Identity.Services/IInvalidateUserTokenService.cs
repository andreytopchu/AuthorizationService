namespace Identity.Services
{
    public interface IInvalidateUserTokenService
    {
        /// <summary>
        /// Инвалидировать токены пользователей, роли или полиси которых были изменены
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvalidateToken(Guid[] userIds, CancellationToken cancellationToken);
    }
}