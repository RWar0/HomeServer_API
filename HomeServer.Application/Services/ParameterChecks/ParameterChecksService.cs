using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Dashboar;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.ParameterChecks;
using HomeServer.Application.Interfaces.Calendar;
using HomeServer.Application.Interfaces.Dashboard;
using HomeServer.Application.Interfaces.ParameterChecks;
using HomeServer.Domain.Enums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.ParameterChecks
{
    public class ParameterChecksService : BaseService, IParameterChecksService, ICalendarEventProvider, IDashboardProvider
    {
        public ParameterChecksService(MyDbContext context, ILogger<ParameterChecksService> logger) : base(context, logger)
        {
        }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateParameterCheckDto createDto)
        {
            try
            {
                if (!CheckIfAnyParameterExistForCreateUpdate(createDto))
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

        public async Task<Result<MessageResponseDto>> CreateForAquariumAsync(Guid aquariumId, CreateParameterCheckForAquariumDto createDto)
        {
            try
            {
                if (!CheckIfAnyParameterExistForCreateUpdate(createDto))
                {
                    return Result.Failure("Należy podać co najmniej jeden parametr (wartości: Ph, Kh, Gh, NO2, NO3, Temperatura)", ProblemType.BadRequest);
                }

                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == aquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                await _context.ParameterChecks.AddAsync(new()
                {
                    AquariumId = aquariumId,
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
                    Message = "Pomiar parametrów został poprawnie utworzony dla akwarium!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create parameter check for aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Nie można utworzyć pomiaru parametrów dla podanego akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<ParameterCheckResponseDto>>> GetAllAsync(PaginationRequestDto paginationDto, ParameterCheckFilterDto filtrationDto)
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
                    .Select(item => new ParameterCheckResponseDto
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

                return Result<PagedResult<ParameterCheckResponseDto>>.Success(new()
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

        public async Task<IEnumerable<CalendarEventDto>> GetCalendarEventsAsync(DateFilterDto filterDto, CancellationToken ct)
        {
            var items = await _context.ParameterChecks
                .Where(item => item.MeasuredAt >= filterDto.FromDate && item.MeasuredAt <= filterDto.ToDate)
                .Select(item => new CalendarEventDto
                {
                    Id = item.Id,
                    Title = "Pomiar parametrów",
                    Description = $"Pomiar parametrów w akwarium: {item.Aquarium.Name}. \n" +
                        $"Dane: \n" +
                        $"Ph: {GetValueOrDash(item.Ph)} \n" +
                        $"Gh: {GetValueOrDash(item.Gh)} \n" +
                        $"Kh: {GetValueOrDash(item.Kh)} \n" +
                        $"No2: {GetValueOrDash(item.No2)} \n" +
                        $"No3: {GetValueOrDash(item.No3)} \n" +
                        $"Temperatura: {GetValueOrDash(item.Temperature)}",
                    Location = $"Akwarium {item.Aquarium.Name}",
                    Date = item.MeasuredAt,
                    Time = null,
                    Category = CalendarEventCategoryEnum.Aquarium.ToString(),
                    SubCategory = CalendarEventSubCategoryEnum.ParameterCheck.ToString(),
                    IsCustomEvent = false
                })
                .ToListAsync(ct);

            return items;
        }

        public async Task<Result<ParameterCheckDto>> GetByIdAsync(Guid parameterCheckId)
        {
            try
            {
                var result = await _context.ParameterChecks.Where(item => item.Id == parameterCheckId)
                    .Select(item => new ParameterCheckDto
                    {
                        Id = parameterCheckId,
                        MeasuredAt = item.MeasuredAt,
                        Ph = item.Ph,
                        Gh = item.Gh,
                        Kh = item.Kh,
                        No2 = item.No2,
                        No3 = item.No3,
                        Temperature = item.Temperature,
                        AquariumName = item.Aquarium.Name,
                    })
                    .FirstOrDefaultAsync();

                if (result is null)
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

        public async Task<Result<ParameterCheckEditResponseDto>> GetForEditAsync(Guid parameterCheckId)
        {
            try
            {
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

                if (result is null)
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

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid parameterCheckId, UpdateParameterCheckDto updateDto)
        {
            try
            {
                if (!CheckIfAnyParameterExistForCreateUpdate(updateDto))
                {
                    return Result.Failure("Należy podać co najmniej jeden parametr (wartości: Ph, Kh, Gh, NO2, NO3, Temperatura)", ProblemType.BadRequest);
                }

                var parameterCheck = await _context.ParameterChecks.FirstOrDefaultAsync(item => item.Id == parameterCheckId);

                if (parameterCheck is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == updateDto.AquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                parameterCheck.AquariumId = updateDto.AquariumId;
                parameterCheck.Aquarium = aquarium;
                parameterCheck.MeasuredAt = updateDto.MeasuredAt;
                parameterCheck.Temperature = updateDto.Temperature;
                parameterCheck.Ph = updateDto.Ph;
                parameterCheck.Kh = updateDto.Kh;
                parameterCheck.Gh = updateDto.Gh;
                parameterCheck.No2 = updateDto.No2;
                parameterCheck.No3 = updateDto.No3;

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

        public async Task<Result<MessageResponseDto>> UpdateForAquariumAsync(Guid parameterCheckId, UpdateParameterCheckForAquariumDto updateDto)
        {
            try
            {
                if (!CheckIfAnyParameterExistForCreateUpdate(updateDto))
                {
                    return Result.Failure("Należy podać co najmniej jeden parametr (wartości: Ph, Kh, Gh, NO2, NO3, Temperatura)", ProblemType.BadRequest);
                }

                var parameterCheck = await _context.ParameterChecks.FirstOrDefaultAsync(item => item.Id == parameterCheckId);

                if (parameterCheck is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                parameterCheck.MeasuredAt = updateDto.MeasuredAt;
                parameterCheck.Temperature = updateDto.Temperature;
                parameterCheck.Ph = updateDto.Ph;
                parameterCheck.Kh = updateDto.Kh;
                parameterCheck.Gh = updateDto.Gh;
                parameterCheck.No2 = updateDto.No2;
                parameterCheck.No3 = updateDto.No3;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie edytowana dla podanego akwarium!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot edit water change with Id: {WaterChangeId}", parameterCheckId);
                return Result.Failure("Nie można edytować podanej podmiany wody dla wybranego akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid parameterCheckId)
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


        // Dashboard Provider
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var counts = await _context.ParameterChecks
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    TodayEventsCount = g.Count(x => x.MeasuredAt == today),
                    TodayCompletedEventsCount = g.Count(x => x.MeasuredAt == today),
                })
                .FirstOrDefaultAsync(ct);

            return new DashboardSummaryDto
            {
                TodayEventsCount = counts?.TodayEventsCount ?? 0,
                OverdueEventsCount = 0,
                TodayCompletedEventsCount = counts?.TodayCompletedEventsCount ?? 0,
                RemainingDaysToNextEvent = null,
                RemainingEventTitle = null,
            };
        }

        public async Task<IEnumerable<DashboardAttentionItemDto>> GetDashboardAttentionItemsAsync(int limit, CancellationToken ct)
        {
            return [];
        }

        public async Task<IEnumerable<DashboardUpcomingEventDto>> GetDashboardUpcomingEventsAsync(int limit, CancellationToken ct)
        {
            return [];
        }

        public async Task<IEnumerable<DashboardRecentActivityDto>> GetDashboardRecentActivitiesAsync(int limit, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var activities = await _context.ParameterChecks
                .Where(item => item.MeasuredAt <= today)
                .OrderByDescending(item => item.MeasuredAt)
                .Take(limit)
                .Select(item => new DashboardRecentActivityDto
                {
                    Id = item.Id,
                    Category = CalendarEventCategoryEnum.Aquarium.ToString(),
                    Title = $"Podmiana wody w akwarium: {item.Aquarium.Name}",
                    Date = item.MeasuredAt,
                })
                .ToListAsync(ct);

            return activities;
        }


        // Helpers
        private static IQueryable<ParameterCheck> ApplySortingForParameterChecks(IQueryable<ParameterCheck> query, PaginationRequestDto paginationDto)
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

        private static IQueryable<ParameterCheck> ApplyFiltrationForParameterChecks(IQueryable<ParameterCheck> query, ParameterCheckFilterDto filterDto)
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

        private static bool CheckIfAnyParameterExistForCreateUpdate(CreateParameterCheckDto dto)
        {
            return dto.Temperature.HasValue || dto.Ph.HasValue || dto.Kh.HasValue || dto.Gh.HasValue || dto.No2.HasValue || dto.No3.HasValue;
        }

        private static bool CheckIfAnyParameterExistForCreateUpdate(CreateParameterCheckForAquariumDto dto)
        {
            return dto.Temperature.HasValue || dto.Ph.HasValue || dto.Kh.HasValue || dto.Gh.HasValue || dto.No2.HasValue || dto.No3.HasValue;
        }

        private static bool CheckIfAnyParameterExistForCreateUpdate(UpdateParameterCheckDto dto)
        {
            return dto.Temperature.HasValue || dto.Ph.HasValue || dto.Kh.HasValue || dto.Gh.HasValue || dto.No2.HasValue || dto.No3.HasValue;
        }

        private static bool CheckIfAnyParameterExistForCreateUpdate(UpdateParameterCheckForAquariumDto dto)
        {
            return dto.Temperature.HasValue || dto.Ph.HasValue || dto.Kh.HasValue || dto.Gh.HasValue || dto.No2.HasValue || dto.No3.HasValue;
        }

        private static string GetValueOrDash(object? a)
        {
            return a?.ToString() ?? "-";
        }
    }
}
