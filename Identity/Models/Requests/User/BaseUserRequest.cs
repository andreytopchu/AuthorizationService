using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models.Requests.User;

public class BaseUserRequest
{
    /// <summary>
    /// Адрес электронной почты
    /// </summary>
    [Required]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; init; }

    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public Guid RoleId { get; init; }
}