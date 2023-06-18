using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Abstractions.Options;

public class TokenOptions
{
    [Required]
    public TimeSpan InviteTokenLifeDays { get; init; }

    [Required]
    public TimeSpan ForgotTokenLifeDays { get; init; }
}