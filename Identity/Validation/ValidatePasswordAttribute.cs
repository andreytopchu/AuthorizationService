using System.Text.RegularExpressions;
using Identity.Domain.Constants;

// ReSharper disable once CheckNamespace
namespace System.ComponentModel.DataAnnotations;

public sealed class ValidatePasswordAttribute : ValidationAttribute
{
    /*
        https://uibakery.io/regex-library/password-regex-csharp
        Has minimum 8 characters in length. Adjust it by modifying {8,20}
        At least one uppercase English letter. You can remove this condition by removing (?=.*?[A-Z])
        At least one lowercase English letter.  You can remove this condition by removing (?=.*?[a-z])
        At least one digit. You can remove this condition by removing (?=.*?[0-9])
        At least one special character,  You can remove this condition by removing (?=.*?[#?!@$%^&*-])
     */

    private const string Pattern = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$";

    public ValidatePasswordAttribute(string? errorMessage = null)
    {
        ErrorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : ValidationErrors.InvalidPassword;
    }

    public override bool IsValid(object? value)
    {
        return value is string {Length: < 20} password && Regex.IsMatch(password, Pattern);
    }
}