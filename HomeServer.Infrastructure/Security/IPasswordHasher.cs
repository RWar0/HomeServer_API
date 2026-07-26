namespace HomeServer.Infrastructure.Security
{
    public interface IPasswordHasher
    {
        string GetHashedPassword(string password);
        bool IsPasswordValid(string password, string hash);
    }
}
