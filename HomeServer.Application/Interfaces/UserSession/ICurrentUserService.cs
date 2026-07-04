namespace HomeServer.Application.Interfaces.UserSession
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Username { get; }
        bool IsAuthenticated { get; }
    }
}
