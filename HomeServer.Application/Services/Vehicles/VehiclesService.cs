using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Vehicles;
using HomeServer.Application.Interfaces.Vehicles;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Vehicles
{
    public class VehiclesService : BaseService, IVehiclesService
    {
        public VehiclesService(MyDbContext context, ILogger<VehiclesService> logger) : base(context, logger)
        { }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateVehicleDto createDto)
        {
            try
            {
                await _context.Vehicles.AddAsync(new()
                {
                    Brand = createDto.Brand,
                    Model = createDto.Model,
                    Production = createDto.Production,
                    Type = createDto.Type,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Pojazd \"{createDto.Brand} {createDto.Model}\" został poprawnie utworzony!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create vehicle");
                return Result.Failure("Nie można utworzyć pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<VehicleListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto)
        {
            try
            {
                var baseQuery = _context.Vehicles.AsNoTracking();

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForVehicle(baseQuery, paginationDto);

                var waterChanges = await baseQuery
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new VehicleListItemDto
                    {
                        Id = item.Id,
                        Brand = item.Brand,
                        Model = item.Model,
                        Production = item.Production,
                        Type = item.Type.ToString(),
                    })
                    .ToListAsync();

                return Result<PagedResult<VehicleListItemDto>>.Success(new()
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
                _logger.LogError(ex, "Cannot get list of all vehicles");
                return Result.Failure("Nie można pobrać pojazdów z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<VehicleEditDto>> GetForEditAsync(Guid vehicleId)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .Where(item => item.Id == vehicleId)
                    .Select(item => new VehicleEditDto
                    {
                        Id = item.Id,
                        Brand = item.Brand,
                        Model = item.Model,
                        Production = item.Production,
                        Type = item.Type.ToString(),
                    })
                    .FirstOrDefaultAsync();

                if (vehicle is null)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get vehicle for edit with Id: {VehicleId}", vehicleId);
                return Result.Failure("Nie można pobrać pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid vehicleId, UpdateVehicleDto updateDto)
        {
            try
            {
                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(item => item.Id == vehicleId);

                if (vehicle is null)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                vehicle.Brand = updateDto.Brand;
                vehicle.Model = updateDto.Model;
                vehicle.Production = updateDto.Production;
                vehicle.Type = updateDto.Type;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Pojazd został poprawnie zaktualizowany!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update vehicle with Id: {VehicleId}", vehicleId);
                return Result.Failure("Nie można zaktualizować pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid vehicleId)
        {
            try
            {
                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(item => item.Id == vehicleId);

                if (vehicle is null)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                _context.Remove(vehicle);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Pojazd został poprawnie usunięty!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete vehicle with Id: {VehicleId}", vehicleId);
                return Result.Failure("Nie można usunąć pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        // Helpers
        private static IQueryable<Vehicle> ApplySortingForVehicle(IQueryable<Vehicle> query, PaginationRequestDto paginationDto)
        {
            return paginationDto.SortBy?.ToLower() switch
            {
                "brand" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Brand)
                    : query.OrderBy(x => x.Brand),

                "model" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Model)
                    : query.OrderBy(x => x.Model),

                "production" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Production)
                    : query.OrderBy(x => x.Production),

                "type" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Type)
                    : query.OrderBy(x => x.Type),

                _ => query.OrderBy(x => x.Brand)
            };
        }
    }
}
