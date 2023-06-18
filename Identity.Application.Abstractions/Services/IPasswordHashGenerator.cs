namespace Identity.Application.Abstractions.Services
{
    public interface IPasswordHashGenerator
    {
        string MakeHash(string salt, string password);
    }
}