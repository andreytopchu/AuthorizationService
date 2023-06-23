using System;

namespace Identity.Options;

public class RedisConfigurationOptions
{
    /// <summary>
    /// Sentinel, SSDB, Twemproxy
    /// </summary>
    public string? CommandMap { get; init; }

    public string[] EndPoints { get; init; } = Array.Empty<string>();

    public string? Password { get; init; }
}