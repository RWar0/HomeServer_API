using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
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

        public async Task<Result<MessageResponseDto>> CreateWaterChange(WaterChangeCreateEditDto createDto)
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
                    Aquarium = aquarium,
                    Amount = createDto.Amount,
                    ChangeDate = createDto.ChangeDate
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

        public async Task<Result<MessageResponseDto>> CreateWaterChangeForAquarium(Guid aquariumId, WaterChangeCreateEditForAquariumDto createDto)
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
                    Aquarium = aquarium,
                    Amount = createDto.Amount,
                    ChangeDate = createDto.ChangeDate
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

        public async Task<Result<PagedResult<WaterChangesGetResponseDto>>> GetAll(PaginationDto paginationDto)
        {
            try
            {
                var baseQuery = _context.WaterChanges.AsNoTracking();

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForWaterChange(baseQuery, paginationDto);

                var waterChanges = await baseQuery
                    .Include(item => item.Aquarium)
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new WaterChangesGetResponseDto
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        ChangeDate = item.ChangeDate,
                        AquariumId = item.Aquarium.Id,
                        AquariumName = item.Aquarium.Name,
                    })
                    .ToListAsync();

                return Result<PagedResult<WaterChangesGetResponseDto>>.Success(new()
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

        public async Task<Result<WaterChangeForEditResponseDto>> GetForEdit(Guid waterChangeId)
        {
            try
            {
                var result = await _context.WaterChanges
                    .Where(item => item.Id == waterChangeId)
                    .Select(item => new WaterChangeForEditResponseDto
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        ChangeDate = item.ChangeDate,
                        AquariumId = item.Aquarium.Id
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
        
        public async Task<Result<WaterChangeForAquariumEditResponseDto>> GetForAquariumEdit(Guid waterChangeId)
        {
            try
            {
                var result = await _context.WaterChanges
                    .Where(item => item.Id == waterChangeId)
                    .Select(item => new WaterChangeForAquariumEditResponseDto
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

        public async Task<Result<MessageResponseDto>> UpdateWaterChange(Guid waterChangeId, WaterChangeCreateEditDto editDto)
        {
            try
            {
                var waterChange = await _context.WaterChanges.FirstOrDefaultAsync(item => item.Id == waterChangeId);

                if (waterChange is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == editDto.AquariumId);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium z podanym identyfikatorem!", ProblemType.NotFound);
                }

                waterChange.Aquarium = aquarium;
                waterChange.Amount = editDto.Amount;
                waterChange.ChangeDate = editDto.ChangeDate;

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
        
        public async Task<Result<MessageResponseDto>> UpdateWaterChangeOfAquarium(Guid waterChangeId, WaterChangeCreateEditForAquariumDto editDto)
        {
            try
            {
                var waterChange = await _context.WaterChanges.FirstOrDefaultAsync(item => item.Id == waterChangeId);

                if (waterChange is null)
                {
                    return Result.Failure("Nie znaleziono podmiany wody z podanym identyfikatorem!", ProblemType.NotFound);
                }

                waterChange.Amount = editDto.Amount;
                waterChange.ChangeDate = editDto.ChangeDate;

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

        public async Task<Result<MessageResponseDto>> Delete(Guid waterChangeId)
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

        private static IQueryable<WaterChange> ApplySortingForWaterChange(IQueryable<WaterChange> query, PaginationDto paginationDto)
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
    }
}
