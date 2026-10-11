using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Calculators;
using HomeServer.Application.Interfaces.Calculators;
using HomeServer.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Calculators
{
    public class CalculatorsService : BaseService, ICalculatorsService
    {
        public CalculatorsService(MyDbContext context, ILogger<CalculatorsService> logger) : base(context, logger) { }

        public async Task<Result<CalculatorCo2ResponseDto>> GetCalculatedCo2Async(CalculatorCo2RequestDto request)
        {
            try
            {
                double result = 3 * request.Kh * Math.Pow(10, 7 - request.Ph);

                double roundedResult = Math.Round(result, 2);

                return Result.Success(new CalculatorCo2ResponseDto
                {
                    Co2 = roundedResult
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot calculate CO2.");
                return Result.Failure("Nie można obliczyć CO2 dla podanych parametrów.", ProblemType.InternalServerError);
            }
        }
    }
}
