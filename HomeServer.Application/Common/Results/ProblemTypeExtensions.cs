namespace HomeServer.Application.Common.Results
{
    public static class ProblemTypeExtensions
    {
        public static int GetStatusCode(this ProblemType problem)
        {
            return problem switch
            {
                ProblemType.BadRequest => 400,
                ProblemType.NotFound => 404,
                ProblemType.Unauthorized => 401,
                ProblemType.Forbidden => 403,
                ProblemType.Conflict => 409,
                ProblemType.InternalServerError => 500,
                _ => 400
            };
        }

        public static string GetTitle(this ProblemType problem)
        {
            return problem switch
            {
                ProblemType.BadRequest => "Bad Request",
                ProblemType.NotFound => "Not Found",
                ProblemType.Unauthorized => "Unauthorized",
                ProblemType.Forbidden => "Forbidden",
                ProblemType.Conflict => "Conflict",
                ProblemType.InternalServerError => "Internal Server Error",
                _ => "Bad Request"
            };
        }
    }
}