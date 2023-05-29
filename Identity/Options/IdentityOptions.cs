using System.ComponentModel.DataAnnotations;

namespace Identity.Options;

public class IdentityOptions
{
    [Required] public string ProviderName { get; set; } = "default-name";
}