using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.User;

public class UpdatePolicyUseCase : IUseCase<IUpdateUserCommand, UserInfo>
{
    public async Task<UserInfo> Process(IUpdateUserCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}