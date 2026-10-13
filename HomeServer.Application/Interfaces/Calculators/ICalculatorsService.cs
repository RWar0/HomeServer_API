using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Calculators;

namespace HomeServer.Application.Interfaces.Calculators
{
    public interface ICalculatorsService
    {
        public Task<Result<CalculatorCo2ResponseDto>> GetCalculatedCo2Async(CalculatorCo2RequestDto request);
    }
}
