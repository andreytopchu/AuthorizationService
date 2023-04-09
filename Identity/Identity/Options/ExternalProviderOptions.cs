using System.ComponentModel.DataAnnotations;
namespace Identity.Options
{
    public class ExternalProviderOptions
    {
        [Required] public string AppId { get; init; }
        [Required] public string AppSecret { get; init; }
    }
}