namespace HomeServer.Application.Common.Results
{
    public enum ProblemType
    {
        BadRequest,
        NotFound,
        Unauthorized,
        Forbidden,
        Conflict,
        InternalServerError
    }
}
