using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.Repositories.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.ApiResource;

namespace Identity.Application.UseCases.ApiResource;

public class AddApiResourceUseCase : IUseCase<IAddApiResourceCommand, ApiResourceInfo>
{
    private readonly IApiResourceWriteRepository _apiResourceWriteRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddApiResourceUseCase(IApiResourceWriteRepository apiResourceWriteRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _apiResourceWriteRepository = apiResourceWriteRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResourceInfo> Process(IAddApiResourceCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        if (await _apiResourceWriteRepository.Read.SingleOrDefaultAsync(new ApiResourceByNameSpecification(arg.Name), cancellationToken) is not null)
        {
            throw new EntityAlreadyExistsException<IdentityServer4.EntityFramework.Entities.ApiResource>(arg.Name);
        }

        var apiResource = _mapper.Map<Identity.Domain.Entities.ApiResource>(arg);

        await _apiResourceWriteRepository.AddAsync(apiResource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetApiResourceById(apiResource.Id, cancellationToken);
    }

    private async Task<ApiResourceInfo> GetApiResourceById(int id, CancellationToken cancellationToken)
    {
        return await _apiResourceWriteRepository.Read.GetApiResourceById<ApiResourceInfo>(id, cancellationToken);
    }
}