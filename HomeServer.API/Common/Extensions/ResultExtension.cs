using HomeServer.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Common.Extensions
{
    public static class ResultExtension
    {
        public static ActionResult ToActionResult<T>(
            this Result<T> result,
            ControllerBase controller)
            where T : class
        {
            return result.IsSuccess
                ? controller.Ok(result.Value)
                : controller.Problem(
                    detail: result.Error,
                    statusCode: result.ErrorCode,
                    title: result.ErrorTitle
                );
        }
    }
}