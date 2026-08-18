namespace HomeServer.Infrastructure.Security
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Method to get hashed password for a given password.
        /// </summary>
        /// <param name="password">Raw password to be hashed</param>
        /// <returns>Hashed password (string)</returns>
        string GetHashedPassword(string password);
        /// <summary>
        /// Method to check if provided password is valid for a given hash.
        /// </summary>
        /// <param name="password">Raw password to be checked</param>
        /// <param name="hash">Hashed password</param>
        /// <returns>True if passwords are exact, otherwise false</returns>
        bool IsPasswordValid(string password, string hash);
    }
}
