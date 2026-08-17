using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.ParameterChecks;
using HomeServer.Application.Interfaces.ParameterChecks;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.ParameterChecks
{
    public class ParameterChecksService : BaseService, IParametersCheckService
    {
        public ParameterChecksService(MyDbContext context, ILogger<ParameterChecksService> logger) : base(context, logger)
        {
        }

        public async Task<Result<MessageResponseDto>> Create(ParameterCheckCreateEditDto createDto)
        {
            try
            {
                if(!CheckIfAnyParameterExistForCreateEdit(createDto))
                {
                    return Result.Failure("Należy podać co najmniej jeden parametr (wartości: Ph, Kh, Gh, NO2, NO3, Temperatura)", ProblemType.BadRequest);
                }

                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == createDto.AquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                await _context.ParameterChecks.AddAsync(new()
                {
                    AquariumId = createDto.AquariumId,
                    Aquarium = aquarium,
                    MeasuredAt = createDto.MeasuredAt,
                    Temperature = createDto.Temperature,
                    Ph = createDto.Ph,
                    Kh = createDto.Kh,
                    Gh = createDto.Gh,
                    No2 = createDto.No2,
                    No3 = createDto.No3,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Pomiar parametrów został poprawnie utworzony!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create parameter check");
                return Result.Failure("Nie można utworzyć pomiaru parametrów dla podanego akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<ParameterChecksGetResponseDto>>> GetAll(PaginationDto paginationDto, ParameterChecksListFiltrationDto filtrationDto)
        {
            try
            {
                var baseQuery = _context.ParameterChecks.AsNoTracking();

                baseQuery = ApplyFiltrationForParameterChecks(baseQuery, filtrationDto);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForParameterChecks(baseQuery, paginationDto);

                var parameterChecks = await baseQuery
                    .Include(item => item.Aquarium)
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new ParameterChecksGetResponseDto
                    {
                        Id = item.Id,
                        MeasuredAt = item.MeasuredAt,
                        Ph = item.Ph,
                        Gh = item.Gh,
                        Kh = item.Kh,
                        No2 = item.No2,
                        No3 = item.No3,
                        Temperature = item.Temperature,
                        AquariumId = item.Aquarium.Id,
                        AquariumName = item.Aquarium.Name,
                    })
                    .ToListAsync();

                return Result<PagedResult<ParameterChecksGetResponseDto>>.Success(new()
                {
                    Data = parameterChecks,
                    Pagination = new()
                    {
                        Page = paginationDto.Page,
                        PageSize = paginationDto.PageSize,
                        TotalCount = totalCount,
                        SortBy = paginationDto.SortBy,
                        SortDirection = paginationDto.SortDirection,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get list of all parameter checks");
                return Result.Failure("Można pobrać pomiarów parametrów z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<ParameterCheckEditResponseDto>> GetForEdit(Guid parameterCheckId)
        {
            try {
                var result = await _context.ParameterChecks.Where(item => item.Id == parameterCheckId)
                    .Select(item => new ParameterCheckEditResponseDto
                    {
                        Id = parameterCheckId,
                        AquariumId = item.AquariumId,
                        MeasuredAt = item.MeasuredAt,
                        Ph = item.Ph,
                        Gh = item.Gh,
                        Kh = item.Kh,
                        No2 = item.No2,
                        No3 = item.No3,
                        Temperature = item.Temperature,
                    })
                    .FirstOrDefaultAsync();

                if(result is null)
                {
                    return Result.Failure("Nie znaleziono podanego pomiaru parametrów", ProblemType.NotFound);
                }

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get parameter check with Id: {ParameterCheckId}", parameterCheckId);
                return Result.Failure("Nie można pobrać podanego pomiaru parametrów!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> Update(Guid parameterCheckId, ParameterCheckCreateEditDto editDto)
        {
            try
            {
                if (!CheckIfAnyParameterExistForCreateEdit(editDto))
                {
                    return Result.Failure("Należy podać co najmniej jeden parametr (wartości: Ph, Kh, Gh, NO2, NO3, Temperatura)", ProblemType.BadRequest);
                }

                var parameterCheck = await _context.ParameterChecks.FirstOrDefaultAsync(item => item.Id == parameterCheckId);

                if (parameterCheck is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == editDto.AquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                parameterCheck.AquariumId = editDto.AquariumId;
                parameterCheck.Aquarium = aquarium;
                parameterCheck.MeasuredAt = editDto.MeasuredAt;
                parameterCheck.Temperature = editDto.Temperature;
                parameterCheck.Ph = editDto.Ph;
                parameterCheck.Kh = editDto.Kh;
                parameterCheck.Gh = editDto.Gh;
                parameterCheck.No2 = editDto.No2;
                parameterCheck.No3 = editDto.No3;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie edytowana!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot edit water change with Id: {WaterChangeId}", parameterCheckId);
                return Result.Failure("Nie można edytować podanej podmiany wody!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> Delete(Guid parameterCheckId)
        {
            try
            {
                var parameterCheck = await _context.ParameterChecks.FirstOrDefaultAsync(item => item.Id == parameterCheckId);

                if (parameterCheck is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                _context.Remove(parameterCheck);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie usunięta!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete parameter check with Id: {ParameterCheckId}", parameterCheckId);
                return Result.Failure("Można usunąć pomiaru parametrów z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        private static IQueryable<ParameterCheck> ApplySortingForParameterChecks(IQueryable<ParameterCheck> query, PaginationDto paginationDto)
        {
            return paginationDto.SortBy?.ToLower() switch
            {
                "aquariumname" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Aquarium.Name)
                    : query.OrderBy(x => x.Aquarium.Name),

                "measuredat" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.MeasuredAt)
                    : query.OrderBy(x => x.MeasuredAt),

                _ => query.OrderByDescending(x => x.MeasuredAt)
            };
        }

        private static IQueryable<ParameterCheck> ApplyFiltrationForParameterChecks(IQueryable<ParameterCheck> query, ParameterChecksListFiltrationDto filterDto)
        {
            if (filterDto.AquariumId.HasValue)
            {
                query = query.Where(item => item.AquariumId == filterDto.AquariumId.Value);
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(item => item.MeasuredAt > filterDto.FromDate.Value);
            }

            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(item => item.MeasuredAt <= filterDto.ToDate.Value.AddDays(1));
            }

            return query;
        }

        private static bool CheckIfAnyParameterExistForCreateEdit(ParameterCheckCreateEditDto dto)
        {
            return dto.Temperature.HasValue || dto.Ph.HasValue || dto.Kh.HasValue || dto.Gh.HasValue || dto.No2.HasValue || dto.No3.HasValue;
        }
    }
}
