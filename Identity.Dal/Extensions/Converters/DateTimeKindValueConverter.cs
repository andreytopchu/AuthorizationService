// ReSharper properties

using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Dal.Extensions.Converters;

internal sealed class DateTimeKindValueConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeKindValueConverter(DateTimeKind kind, ConverterMappingHints? mappingHints = null)
        : base(
            v => v.ToUniversalTime(), // Что-бы в timestamp дата всегда хранилась в UTC.
            v => DateTime.SpecifyKind(v, kind)
                .ToLocalTime(), // timestamp в базе эквивалентен `Unspecified DateTime` поэтому просто восстанавливаем заведомо известный Kind.
            mappingHints)
    {
    }
}