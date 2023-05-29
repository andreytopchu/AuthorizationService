namespace Identity.Domain.Constants
{
    public class RoleConstantId
    {
        public static readonly Guid SuperAdminRoleId = new("d5939743-cca4-42eb-9777-36c47b8f08ab");
        public static readonly Guid NoAccessRoleId = new("37aea133-e83c-459e-8404-dbffc7cec4f6");
        public static readonly Guid FullReadRoleId = new("112a5b56-d1b6-464b-85fb-caf1c31db198");
        public static readonly Guid FullWriteRoleId = new("99d300db-0033-44d5-b033-56331a3503ce");
    }
}