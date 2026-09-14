using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Select;
using HomeServer.Application.Dtos.VehicleFuelings;
using HomeServer.Application.Dtos.Vehicles;
using HomeServer.Application.Dtos.VehicleServices;
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

                var vehicles = await baseQuery
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
                    Data = vehicles,
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

        public async Task<Result<VehicleBaseDto>> GetBaseDataAsync(Guid vehicleId)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .Where(item => item.Id == vehicleId)
                    .Select(item => new VehicleBaseDto
                    {
                        Id = item.Id,
                        Brand = item.Brand,
                        Model = item.Model,
                        Production = item.Production,
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
                _logger.LogError(ex, "Cannot get vehicle base data with Id: {VehicleId}", vehicleId);
                return Result.Failure("Nie można pobrać pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
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

        public async Task<Result<SelectOptionDto[]>> GetForSelectAsync()
        {
            try
            {
                var vehicles = await _context.Vehicles
                    .AsNoTracking()
                    .Select(item => new SelectOptionDto
                    {
                        Id = item.Id,
                        Name = $"{item.Brand} {item.Model}"
                    })
                    .ToListAsync();

                return Result.Success(vehicles.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get list for select of all vehicles");
                return Result.Failure("Nie można pobrać listy pojazdów do wyboru z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<VehicleFuelingsForVehicleListItemDto>>> GetFuelingsAsync(Guid vehicleId, PaginationRequestDto paginationDto, DateFilterDto filterDto)
        {
            try
            {
                var baseQuery = _context.VehicleFuelings
                    .AsNoTracking()
                    .Where(item => item.VehicleId == vehicleId);

                baseQuery = ApplyFiltrationForVehicleFueling(baseQuery, filterDto);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForVehicleFuelings(baseQuery, paginationDto);

                var fuelings = await baseQuery
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(fueling => new VehicleFuelingsForVehicleListItemDto
                    {
                        Id = fueling.Id,
                        Cost = fueling.Cost,
                        Date = fueling.Date,
                        Quantity = fueling.Quantity,
                    })
                    .ToListAsync();
                
                return Result<PagedResult<VehicleFuelingsForVehicleListItemDto>>.Success(new()
                {
                    Data = fuelings,
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
                _logger.LogError(ex, "Cannot get fuelings for vehicle with Id: {VehicleId}", vehicleId);
                return Result.Failure("Nie można pobrać tankowań pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<VehicleServiceForVehicleListItemDto>>> GetServicesAsync(Guid vehicleId, PaginationRequestDto paginationDto, VehicleServiceForVehicleFilterDto filterDto)
        {
            try
            {
                var baseQuery = _context.VehicleServices
                    .AsNoTracking()
                    .Where(item => item.VehicleId == vehicleId);

                baseQuery = ApplyFiltrationForVehicleService(baseQuery, filterDto);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForVehicleServices(baseQuery, paginationDto);

                var services = await baseQuery
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(service => new VehicleServiceForVehicleListItemDto
                    {
                        Id = service.Id,
                        Title = service.Title,
                        Cost = service.Cost,
                        Mileage = service.Mileage,
                        Date = service.Date,
                        ItemsCount = service.Items.Count()
                    })
                    .ToListAsync();
                
                return Result<PagedResult<VehicleServiceForVehicleListItemDto>>.Success(new()
                {
                    Data = services,
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
                _logger.LogError(ex, "Cannot get fuelings for vehicle with Id: {VehicleId}", vehicleId);
                return Result.Failure("Nie można pobrać tankowań pojazdu!", ProblemType.InternalServerError);
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

        private static IQueryable<VehicleFueling> ApplySortingForVehicleFuelings(IQueryable<VehicleFueling> query, PaginationRequestDto paginationDto)
        {
            return paginationDto.SortBy?.ToLower() switch
            {
                "date" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Date)
                    : query.OrderBy(x => x.Date),

                "cost" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Cost)
                    : query.OrderBy(x => x.Cost),

                "quantity" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Quantity)
                    : query.OrderBy(x => x.Quantity),

                _ => query.OrderByDescending(x => x.Date),
            };
        }

        private static IQueryable<VehicleService> ApplySortingForVehicleServices(IQueryable<VehicleService> query, PaginationRequestDto paginationDto)
        {
            return paginationDto.SortBy?.ToLower() switch
            {
                "title" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Title)
                    : query.OrderBy(x => x.Title),

                "date" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Date)
                    : query.OrderBy(x => x.Date),

                "cost" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Cost)
                    : query.OrderBy(x => x.Cost),

                "mileage" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Mileage)
                    : query.OrderBy(x => x.Mileage),

                "vehicleid" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.VehicleId)
                    : query.OrderBy(x => x.VehicleId),

                _ => query.OrderByDescending(x => x.Date),
            };
        }

        private static IQueryable<VehicleFueling> ApplyFiltrationForVehicleFueling(IQueryable<VehicleFueling> query, DateFilterDto filterDto)
        {
            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(item => item.Date > filterDto.FromDate.Value);
            }

            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(item => item.Date <= filterDto.ToDate.Value.AddDays(1));
            }

            return query;
        }

        private static IQueryable<VehicleService> ApplyFiltrationForVehicleService(IQueryable<VehicleService> query, VehicleServiceForVehicleFilterDto filterDto)
        {
            if (!string.IsNullOrWhiteSpace(filterDto.Title))
            {
                query = query.Where(v => EF.Functions.ILike(v.Title, $"%{filterDto.Title}%"));
            }

            if (filterDto.FromDate.HasValue)
            {
                query = query.Where(item => item.Date > filterDto.FromDate.Value);
            }

            if (filterDto.ToDate.HasValue)
            {
                query = query.Where(item => item.Date <= filterDto.ToDate.Value.AddDays(1));
            }

            return query;
        }

    }
}
