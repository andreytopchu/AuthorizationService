using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Identity.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProfileService : IProfileService
    {
        private readonly IEnumerable<UserStore.UserStoreClient> _clients;

        public ProfileService(IEnumerable<UserStore.UserStoreClient> clients)
        {
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.RequestedClaimTypes.Any())
            {
                var subject = new Subject
                {
                    Sub = context.Subject.GetSubjectId(),
                };

                subject.Claims.AddRange(context.Subject.Claims.Select(x => new SimpleClaim {Type = x.Type, Value = x.Value}));
                subject.Claims.AddRange(context.RequestedClaimTypes.Except(subject.Claims.Select(x => x.Type))
                    .Select(x => new SimpleClaim {Type = x}));

                try
                {
                    var user = await GetClient(context.Client.ClientId).FindBySubjectIdAsync(subject);
                    if (user != null)
                    {
                        context.AddRequestedClaims(user.Claims.Select(x => new Claim(x.Type, x.Value)));
                    }
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
                {
                    context.IssuedClaims.Clear();
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var claims = new RepeatedField<SimpleClaim>();
                if (!context.Subject.Claims.IsNullOrEmpty())
                {
                    claims.AddRange(context.Subject.Claims.Select(x => new SimpleClaim {Type = x.Type, Value = x.Value}));
                }

                var subject = new Subject {Sub = context.Subject.GetSubjectId()};
                subject.Claims.Add(claims);

                var user = await GetClient(context.Client.ClientId)
                    .FindBySubjectIdAsync(subject);
                if (user != null)
                {
                    context.IsActive = user.IsActive;
                }
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
            {
                context.IsActive = false;
            }
        }

        private UserStore.UserStoreClient GetClient(string clientId)
        {
            return _clients.First(x => x.Name.StartsWith(clientId));
        }
    }
}