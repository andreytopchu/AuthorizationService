using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.Options;

public class GrpcClientOptions
{
    [Required] public Uri ProfileServiceUri { get; init; }
    [Required] public Uri AdminServiceUri { get; init; }
}