using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Identity.Application.Abstractions.Extensions
{
    public static class StringExtensions
    {
        /// <summary>Бросает исключение если строка оказалась Null.</summary>
        /// <exception cref="ArgumentException"/>
        public static string NotNullParam([NotNull] this string? value, [CallerArgumentExpression("value")] string paramName = "")
        {
            if (value != null)
            {
                return value;
            }

            throw new ArgumentNullException(paramName, "Argument should not be null");
        }

        /// <summary>Бросает исключение если строка оказалась Null или пустой.</summary>
        /// <exception cref="ArgumentException"/>
        public static string NotNullAndNotEmptyParam([NotNull] this string? value, [CallerArgumentExpression("value")] string paramName = "",
            string? message = null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            throw new ArgumentException(message ?? "Argument should not be null or empty", paramName);
        }
    }
}