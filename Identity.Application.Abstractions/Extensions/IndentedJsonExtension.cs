using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Identity.Application.Abstractions.Extensions
{
    public static class IndentedJsonExtension
    {
        /// <summary>
        /// Перевод объекта в читаемый JSON.
        /// Используется для записи объектов в текстовые логи.
        /// </summary>
        public static IndentedJson<T> ToFriendlyJson<T>(this T obj)
        {
            return new IndentedJson<T>(obj);
        }

        public class IndentedJson
        {
            protected static readonly JsonSerializerOptions Opt = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        [DebuggerDisplay("{ToString(),nq}")]
        public sealed class IndentedJson<T> : IndentedJson
        {
            private readonly T _obj;

            public IndentedJson(T obj) => _obj = obj;

            public static implicit operator string(IndentedJson<T> self) => self.ToString();

            public override string ToString()
            {
                return _obj != null ? JsonSerializer.Serialize(_obj, Opt) : string.Empty;
            }
        }
    }
}