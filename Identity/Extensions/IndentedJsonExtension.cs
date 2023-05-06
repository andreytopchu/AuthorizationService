using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Identity.Extensions
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

#pragma warning disable CA1034
        public class IndentedJson
        {
            protected static readonly JsonSerializerOptions Opt = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
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
#pragma warning restore CA1034
    }
}