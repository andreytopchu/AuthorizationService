// ReSharper properties

using System.Diagnostics;
using Identity.Application.Abstractions.Extensions;

namespace Identity.Application.Abstractions.UseCases;

public interface IUseCase
{
    public Guid CorrelationId => Activity.Current.GetTraceId();
}

public interface IUseCase<in TArg> : IUseCase
    where TArg : IUseCaseArg
{
    Task Process(TArg arg, CancellationToken cancellationToken);
}

public interface IUseCase<in TArg, TResult> : IUseCase
    where TArg : IUseCaseArg
{
    Task<TResult> Process(TArg arg, CancellationToken cancellationToken);
}