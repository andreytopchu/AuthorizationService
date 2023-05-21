namespace Identity.Application.Abstractions.Models.Authorization;

public class AuthResult
{
    public bool Success { get; init; }
    public string Message { get; init; }
    public string Sub { get; init; }
    public uint NoUntil { get; init; }
    public AuthErrorCode ErrorCode { get; init; }
}