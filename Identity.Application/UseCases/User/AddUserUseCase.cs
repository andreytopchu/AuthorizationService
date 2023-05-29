using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.User;

public class AddUserUseCase : IUseCase<IAddUserCommand, UserInfo>
{
    public async Task<UserInfo> Process(IAddUserCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}