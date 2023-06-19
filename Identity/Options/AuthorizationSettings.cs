using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Identity.Options
{
    public class AuthorizationSettings
    {
        /// <summary>
        /// Обязательно. Адрес сервера выдавшего токен.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public Uri AuthorityUrl { get; set; }

        /// <summary>
        /// Обязательно. Имя ApiResource
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string ApiResource { get; set; }

        /// <summary>
        /// TokenType = Reference. Обязательно, если используется Introspection endpoint
        /// </summary>
        public string ApiResourceSecret { get; set; }

        /// <summary>
        /// TokenType = Reference. Кэширование ответа от Introspection.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public int IntrospectionCacheTimeSeconds { get; set; } = 60;

        /// <summary>
        /// TokenType = JWT. Опционально. Требовать наличие одного перечисленных scope в токене
        /// [Authorize("ApiScopeRequired")]
        /// </summary>
        public string[]? ApiScopeRequired { get; set; }

        /// <summary>
        /// TokenType = JWT. Опционально. Требовать указанного scope в токене
        /// [Authorize("only-scope-api")]
        /// </summary>
        public string[]? ApiScopeOnlyRequired { get; set; }

        /// <summary>
        /// TokenType = JWT. Опционально. На каждый элемент массива, создает Policy: ([name].read, [name].write)
        /// используем в коде как [Authorize("story.write")]
        /// </summary>
        public string[] ApiPolicies { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Включает использования Reference токенов вместо JWT. "Редирект" на интроспекцию Reference-токена
        /// происходит при помощи Selector.ForwardReferenceToken, ориентирующегося на наличие . в значение токена
        /// </summary>
        public bool UseReferenceToken { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(AuthorityUrl.ToString()))
                throw new InvalidDataException("AuthorityUrl can't be null");

            if (string.IsNullOrWhiteSpace(ApiResource))
                throw new InvalidDataException("ApiResource can't be null");
        }
    }
}