using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Identity.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProfileService : IProfileService
    {
        private readonly IUserStoreService _userStoreService;

        public ProfileService(IUserStoreService userStoreService)
        {
            _userStoreService = userStoreService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.RequestedClaimTypes.Any())
            {
                var subject = new Subject
                {
                    Sub = context.Subject.GetSubjectId(),
                };

                subject.Claims.AddRange(context.Subject.Claims.Select(x => new SimpleClaim
                    { Type = x.Type, Value = x.Value }));
                subject.Claims.AddRange(context.RequestedClaimTypes.Except(subject.Claims.Select(x => x.Type))
                    .Select(x => new SimpleClaim { Type = x }));

                var user = await _userStoreService.FindBySubjectId(subject);
                if (user != null)
                {
                    context.AddRequestedClaims(user.Claims.Select(x => new Claim(x.Type, x.Value)));
                }
                else
                {
                    context.IssuedClaims.Clear();
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userStoreService.FindBySubjectId(new Subject { Sub = context.Subject.GetSubjectId() });
            context.IsActive = user is not null && user.IsActive;
        }
    }
}