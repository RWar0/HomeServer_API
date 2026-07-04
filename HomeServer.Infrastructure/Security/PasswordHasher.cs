namespace HomeServer.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GetHashedPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool IsPasswordValid(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
