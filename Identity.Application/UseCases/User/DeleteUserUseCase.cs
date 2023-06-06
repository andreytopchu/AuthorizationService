using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.UseCases.User;

public class DeleteUserUseCase : IUseCase<IDeleteUserCommand>
{
    public async Task Process(IDeleteUserCommand arg, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}