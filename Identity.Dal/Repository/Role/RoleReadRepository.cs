using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.Role;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository.Role;

public class RoleReadRepository : GenericReadRepository<Domain.Entities.Role, Guid>, IRoleReadRepository
{
    private readonly IMapper _mapper;
    protected override IQueryable<Domain.Entities.Role> BaseQuery => base.BaseQuery.Include(x => x.Policies);

    public RoleReadRepository(IReadDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public async Task<bool> IsRoleExistAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await FirstOrDefaultAsync(new ActiveRoleSpecification(id), cancellationToken);

        return role is not null;
    }

    public async Task<TInfo> GetRoleByIdAsync<TInfo>(Guid roleId, CancellationToken cancellationToken)
    {
        var roleInfo = await QueryBy(new ActiveRoleSpecification(roleId))
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (roleInfo == null)
            throw new RoleNotFoundException(roleId);

        return roleInfo;
    }
}