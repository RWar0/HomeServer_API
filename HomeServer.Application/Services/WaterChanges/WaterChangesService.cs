using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.WaterChanges;
using HomeServer.Application.Interfaces.WaterChanges;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.WaterChanges
{
    public class WaterChangesService : BaseService, IWaterChangesService
    {
        public WaterChangesService(MyDbContext context, ILogger<WaterChangesService> logger) : base(context, logger) { }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateWaterChangeDto createDto)
        {
            try
            {
                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == createDto.AquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                await _context.WaterChanges.AddAsync(new()
                {
                    Amount = createDto.Amount,
                    ChangeDate = createDto.ChangeDate,
                    AquariumId = createDto.AquariumId,
                    Aquarium = aquarium,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie utworzona!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create water change");
                return Result.Failure("Nie można utworzyć podmiany wody dla podanego akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> CreateForAquariumAsync(Guid aquariumId, CreateWaterChangeForAquariumDto createDto)
        {
            try
            {
                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == aquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                await _context.WaterChanges.AddAsync(new()
                {
                    Amount = createDto.Amount,
                    ChangeDate = createDto.ChangeDate,
                    AquariumId = aquariumId,
                    Aquarium = aquarium,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie utworzona!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create water change for aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Nie można utworzyć podmiany wody dla podanego akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<WaterChangeListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto, WaterChangeFilterDto filterDto)
        {
            try
            {
                var baseQuery = _context.WaterChanges.AsNoTracking();

                baseQuery = ApplyFiltrationForWaterChange(baseQuery, filterDto);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForWaterChange(baseQuery, paginationDto);

                var waterChanges = await baseQuery
                    .Include(item => item.Aquarium)
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new WaterChangeListItemDto
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        ChangeDate = item.ChangeDate,
                        AquariumId = item.Aquarium.Id,
                        AquariumName = item.Aquarium.Name,
                    })
                    .ToListAsync();

                return Result<PagedResult<WaterChangeListItemDto>>.Success(new()
                {
                    Data = waterChanges,
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
                _logger.LogError(ex, "Cannot get list of all water changes");
                return Result.Failure("Można pobrać podmian wody z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<WaterChangeEditDto>> GetForEditAsync(Guid waterChangeId)
        {
            try
            {
                var result = await _context.WaterChanges
                    .Where(item => item.Id == waterChangeId)
                    .Select(item => new WaterChangeEditDto
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        ChangeDate = item.ChangeDate,
                        AquariumId = item.AquariumId
                    })
                    .FirstOrDefaultAsync();

                if (result is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get water change with Id: {WaterChangeId}", waterChangeId);
                return Result.Failure("Nie można pobrać podanej podmiany wody!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<WaterChangeEditForAquariumDto>> GetForAquariumEditAsync(Guid waterChangeId)
        {
            try
            {
                var result = await _context.WaterChanges
                    .Where(item => item.Id == waterChangeId)
                    .Select(item => new WaterChangeEditForAquariumDto
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        ChangeDate = item.ChangeDate
                    })
                    .FirstOrDefaultAsync();

                if (result is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get water change with Id: {WaterChangeId}", waterChangeId);
                return Result.Failure("Nie można pobrać podanej podmiany wody!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid waterChangeId, UpdateWaterChangeDto updateDto)
        {
            try
            {
                var waterChange = await _context.WaterChanges.FirstOrDefaultAsync(item => item.Id == waterChangeId);

                if (waterChange is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == updateDto.AquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                waterChange.AquariumId = updateDto.AquariumId;
                waterChange.Aquarium = aquarium;
                waterChange.Amount = updateDto.Amount;
                waterChange.ChangeDate = updateDto.ChangeDate;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie edytowana!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot edit water change with Id: {WaterChangeId}", waterChangeId);
                return Result.Failure("Nie można edytować podanej podmiany wody!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateForAquariumAsync(Guid waterChangeId, UpdateWaterChangeForAquariumDto updateDto)
        {
            try
            {
                var waterChange = await _context.WaterChanges.FirstOrDefaultAsync(item => item.Id == waterChangeId);

                if (waterChange is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                waterChange.Amount = updateDto.Amount;
                waterChange.ChangeDate = updateDto.ChangeDate;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie edytowana!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot edit water change with Id: {WaterChangeId}", waterChangeId);
                return Result.Failure("Nie można edytować podanej podmiany wody!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid waterChangeId)
        {
            try
            {
                var waterChange = await _context.WaterChanges.FirstOrDefaultAsync(item => item.Id == waterChangeId);

                if (waterChange is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                _context.Remove(waterChange);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Podmiana wody została poprawnie usunięta!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete water change with Id: {WaterChangeId}", waterChangeId);
                return Result.Failure("Można usunąć podmiany wody z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        private static IQueryable<WaterChange> ApplySortingForWaterChange(IQueryable<WaterChange> query, PaginationRequestDto paginationDto)
        {
            return paginationDto.SortBy?.ToLower() switch
            {
                "amount" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Amount)
                    : query.OrderBy(x => x.Amount),

                "changedate" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.ChangeDate)
                    : query.OrderBy(x => x.ChangeDate),
                "aquariumname" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Aquarium.Name)
                    : query.OrderBy(x => x.Aquarium.Name),

                _ => query.OrderByDescending(x => x.ChangeDate)
            };
        }

        private static IQueryable<WaterChange> ApplyFiltrationForWaterChange(IQueryable<WaterChange> query, WaterChangeFilterDto filterDto)
        {
            if (filterDto.AquariumId.HasValue)
            {
                query = query.Where(item => item.AquariumId == filterDto.AquariumId.Value);
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(item => item.ChangeDate > filterDto.FromDate.Value);
            }

            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(item => item.ChangeDate <= filterDto.ToDate.Value.AddDays(1));
            }

            return query;
        }
    }
}
