using Grpc.Core;
using gRPC.UserStore;
using Shared.HealthCheck;

namespace Identity.HealthCheck
{
    public class UserStoreHealthClient : BaseHealthClient
    {
        public UserStoreHealthClient(CallInvoker invoker) : base(invoker)
        {
        }

        public override string[] NameServices { get; } = { UserStoreHealthService.ServiceName };
    }
}