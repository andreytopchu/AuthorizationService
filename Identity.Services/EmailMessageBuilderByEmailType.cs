// ReSharper properties

using Dex.SecurityTokenProvider.Interfaces;
using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Tokens;
using Identity.Application.Abstractions.Options;
using Identity.Application.Abstractions.Services;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Identity.Services;

public class EmailMessageBuilderByEmailType : IEmailMessageBuilderByEmailType
{
    private const string TemplateFolder = "Templates";
    private readonly IOptions<TokenOptions> _tokenOptions;
    private readonly ITokenProvider _tokenProvider;

    public EmailMessageBuilderByEmailType(IOptions<TokenOptions> tokenOptions, ITokenProvider tokenProvider)
    {
        _tokenOptions = tokenOptions ?? throw new ArgumentNullException(nameof(tokenOptions));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    public async Task<(string subject, string body)> Build(Uri baseUri, EmailType emailType, User user, CancellationToken cancellation)
    {
        if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
        if (user == null) throw new ArgumentNullException(nameof(user));

        TimeSpan timeout;
        string token;

        switch (emailType)
        {
            case EmailType.RegisterUser:
                timeout = _tokenOptions.Value.InviteTokenLifeDays;
                token = await CreateTokenAsUrlAsync<RegisterUserToken>(user, timeout, cancellation);
                return await CreateRegisterMessageBody(baseUri, user, token, cancellation);
            case EmailType.RecoveryPassword:
                timeout = _tokenOptions.Value.ForgotTokenLifeDays;
                token = await CreateTokenAsUrlAsync<RecoveryUserPasswordToken>(user, timeout, cancellation);
                return await CreateRecoveryPasswordMessageBody(baseUri, user, token, cancellation);

            default:
                throw new ArgumentOutOfRangeException(nameof(emailType), emailType, null);
        }
    }

    private async Task<string> CreateTokenAsUrlAsync<T>(User user, TimeSpan timeout, CancellationToken cancellation) where T : BaseUserToken
    {
        return typeof(T) switch
        {
            not null when typeof(T) == typeof(RegisterUserToken) =>
                await _tokenProvider.CreateTokenAsUrlAsync<RegisterUserToken>(token => { token.UserId = user.Id; }, timeout, cancellation),

            not null when typeof(T) == typeof(RecoveryUserPasswordToken) =>
                await _tokenProvider.CreateTokenAsUrlAsync<RegisterUserToken>(token => { token.UserId = user.Id; }, timeout, cancellation),

            _ => throw new ArgumentOutOfRangeException(nameof(T))
        };
    }

    private static async Task<(string subject, string body)> CreateRegisterMessageBody(Uri baseUri, User user,
        string confirmationToken, CancellationToken cancellation)
    {
        var loginPageUriBuilder = new UriBuilder(baseUri) {Path = "auth/signIn"};
        var linkToLoginPage = Uri.EscapeDataString(loginPageUriBuilder.Uri.ToString());

        var newPassUriBuilder = new UriBuilder(baseUri)
        {
            Path = "auth/invitation",
            Query = "?name=" + user.GetFullName() + "&email=" + user.Email + "&token=" + confirmationToken
        };
        var linkForCreatingNewPassword = Uri.EscapeDataString(newPassUriBuilder.Uri.ToString());

        var path = Path.Combine(TemplateFolder, "ActivationEmail.cshtml");
        var template = await File.ReadAllTextAsync(path, cancellation);

        return
        (
            "Приглашение пользователя",
            template.Replace("%user.Name%", user.GetFullName(), StringComparison.OrdinalIgnoreCase)
                .Replace("%linkToLoginPage%", linkToLoginPage, StringComparison.OrdinalIgnoreCase)
                .Replace("%linkForCreatingNewPassword%", linkForCreatingNewPassword, StringComparison.OrdinalIgnoreCase)
        );
    }

    private static async Task<(string subject, string body)> CreateRecoveryPasswordMessageBody(Uri baseUri,
        User user, string confirmationToken, CancellationToken cancellation)
    {
        var recoveryPassUriBuilder = new UriBuilder(baseUri)
        {
            Path = "auth/recoveryPassword",
            Query = "?email=" + user.Email + "&token=" + confirmationToken
        };
        var linkForRecoveryPassword = Uri.EscapeDataString(recoveryPassUriBuilder.Uri.ToString());

        var path = Path.Combine(TemplateFolder, "PasswordRecoveryEmail.cshtml");
        var template = await File.ReadAllTextAsync(path, cancellation);

        return
        (
            "Восстановление пароля",
            template.Replace("%user.Name%", user.GetFullName(), StringComparison.InvariantCulture)
                .Replace("%linkForRecoveryPassword%", linkForRecoveryPassword, StringComparison.InvariantCulture)
        );
    }
}