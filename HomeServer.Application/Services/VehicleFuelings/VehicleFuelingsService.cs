using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.VehicleFuelings;
using HomeServer.Application.Interfaces.Calendar;
using HomeServer.Application.Interfaces.VehicleFuelings;
using HomeServer.Domain.Enums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.VehicleFuelings
{
    public class VehicleFuelingsService : BaseService, IVehicleFuelingsService, ICalendarEventProvider
    {
        public VehicleFuelingsService(MyDbContext context, ILogger<VehicleFuelingsService> logger) : base(context, logger)
        { }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateVehicleFuelingDto createDto)
        {
            try
            {
                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == createDto.VehicleId);

                if (vehicle is null)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                await _context.VehicleFuelings.AddAsync(new()
                {
                    Cost = createDto.Cost,
                    Date = createDto.Date,
                    Quantity = createDto.Quantity,
                    VehicleId = createDto.VehicleId,
                    Vehicle = vehicle,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Tankowanie zostało poprawnie utworzone!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create vehicle fueling!");
                return Result.Failure("Nie można utworzyć tankowania pojazdu!!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> CreateForVehicleAsync(Guid vehicleId, CreateVehicleFuelingForVehicleDto createDto)
        {
            try
            {
                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);

                if (vehicle is null)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                await _context.VehicleFuelings.AddAsync(new()
                {
                    Cost = createDto.Cost,
                    Date = createDto.Date,
                    Quantity = createDto.Quantity,
                    VehicleId = vehicleId,
                    Vehicle = vehicle,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Tankowanie zostało poprawnie utworzone!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create vehicle fueling!");
                return Result.Failure("Nie można utworzyć tankowania pojazdu!!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<VehicleFuelingListItem>>> GetAllAsync(PaginationRequestDto paginationDto, VehicleFuelingFilterDto filterDto)
        {
            try
            {
                var baseQuery = _context.VehicleFuelings.AsQueryable();

                baseQuery = ApplyFiltrationForVehicleFueling(baseQuery, filterDto);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForVehicleFueling(baseQuery, paginationDto);


                var vehicles = await baseQuery
                    .Include(item => item.Vehicle)
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new VehicleFuelingListItem
                    {
                        Id = item.Id,
                        Cost = item.Cost,
                        Date = item.Date,
                        Quantity = item.Quantity,
                        VehicleId = item.VehicleId,
                        VehicleName = $"{item.Vehicle.Brand} {item.Vehicle.Model}",
                    })
                    .ToListAsync();

                return Result<PagedResult<VehicleFuelingListItem>>.Success(new()
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
                _logger.LogError(ex, "Cannot fetch vehicle fuelings!");
                return Result.Failure("Nie można załadować tankowań pojazdów!", ProblemType.InternalServerError);
            }
        }

        public async Task<IEnumerable<CalendarEventDto>> GetCalendarEventsAsync(DateFilterDto filterDto, CancellationToken ct)
        {
            var items = await _context.VehicleFuelings
                .Where(item => item.Date >= filterDto.FromDate && item.Date <= filterDto.ToDate)
                .Select(item => new CalendarEventDto
                {
                    Id = item.Id,
                    Title = "Tankowanie pojazdu",
                    Description = $"Zatankowano {item.Quantity}l do: {item.Vehicle.Brand} {item.Vehicle.Model}.",
                    Location = $"{item.Vehicle.Brand} {item.Vehicle.Model}",
                    Date = item.Date,
                    Time = null,
                    Category = CalendarEventCategoryEnum.Vehicle.ToString(),
                    SubCategory = CalendarEventSubCategoryEnum.VehicleFueling.ToString(),
                    IsCustomEvent = false
                })
                .ToListAsync(ct);

            return items;
        }

        public async Task<Result<VehicleFuelingDto>> GetById(Guid vehicleFuelingId)
        {
            try
            {
                var vehicleFueling = await _context.VehicleFuelings
                    .Where(item => item.Id == vehicleFuelingId)
                    .Select(item => new VehicleFuelingDto
                    {
                        Id = item.Id,
                        Cost = item.Cost,
                        Date = item.Date,
                        Quantity = item.Quantity,
                        VehicleName = $"{item.Vehicle.Brand} {item.Vehicle.Model}"
                    })
                    .FirstOrDefaultAsync();

                if (vehicleFueling is null)
                {
                    return Result.Failure("Nie znaleziono tankowania pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(vehicleFueling);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get vehicle fueling for edit with Id: {VehicleFuelingId}", vehicleFuelingId);
                return Result.Failure("Nie można pobrać tankowania pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<VehicleFuelingEditDto>> GetForEditAsync(Guid vehicleFuelingId)
        {
            try
            {
                var vehicleFueling = await _context.VehicleFuelings
                    .Where(item => item.Id == vehicleFuelingId)
                    .Select(item => new VehicleFuelingEditDto
                    {
                        Id = item.Id,
                        Cost = item.Cost,
                        Date = item.Date,
                        Quantity = item.Quantity,
                        VehicleId = item.VehicleId
                    })
                    .FirstOrDefaultAsync();

                if (vehicleFueling is null)
                {
                    return Result.Failure("Nie znaleziono tankowania pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(vehicleFueling);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get vehicle fueling for edit with Id: {VehicleFuelingId}", vehicleFuelingId);
                return Result.Failure("Nie można pobrać tankowania pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<VehicleFuelingWithoutVehicleEditDto>> GetForEditWithoutVehicleAsync(Guid vehicleFuelingId)
        {
            try
            {
                var vehicleFueling = await _context.VehicleFuelings
                    .Where(item => item.Id == vehicleFuelingId)
                    .Select(item => new VehicleFuelingWithoutVehicleEditDto
                    {
                        Id = item.Id,
                        Cost = item.Cost,
                        Date = item.Date,
                        Quantity = item.Quantity,
                    })
                    .FirstOrDefaultAsync();

                if (vehicleFueling is null)
                {
                    return Result.Failure("Nie znaleziono tankowania pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(vehicleFueling);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get vehicle fueling for edit with Id: {VehicleFuelingId}", vehicleFuelingId);
                return Result.Failure("Nie można pobrać tankowania pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid vehicleFuelingId, UpdateVehicleFuelingDto updateDto)
        {
            try
            {
                var vehicleFueling = await _context.VehicleFuelings.FirstOrDefaultAsync(item => item.Id == vehicleFuelingId);

                if (vehicleFueling is null)
                {
                    return Result.Failure("Nie znaleziono tankowania z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == updateDto.VehicleId);

                if (vehicle is null)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                vehicleFueling.Cost = updateDto.Cost;
                vehicleFueling.Date = updateDto.Date;
                vehicleFueling.Quantity = updateDto.Quantity;
                vehicleFueling.VehicleId = updateDto.VehicleId;
                vehicleFueling.Vehicle = vehicle;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Tankowanie zostało zaktualizowane!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update vehicle fueling with Id: {VehicleFuelingId}", vehicleFuelingId);
                return Result.Failure("Nie można zaktualizować tankowania pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateForVehicleAsync(Guid vehicleFuelingId, UpdateVehicleFuelingForVehicleDto updateDto)
        {
            try
            {
                var vehicleFueling = await _context.VehicleFuelings.FirstOrDefaultAsync(item => item.Id == vehicleFuelingId);

                if (vehicleFueling is null)
                {
                    return Result.Failure("Nie znaleziono tankowania z podanym identyfikatorem!", ProblemType.NotFound);
                }

                vehicleFueling.Cost = updateDto.Cost;
                vehicleFueling.Date = updateDto.Date;
                vehicleFueling.Quantity = updateDto.Quantity;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Tankowanie zostało zaktualizowane!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update vehicle fueling with Id: {VehicleFuelingId}", vehicleFuelingId);
                return Result.Failure("Nie można zaktualizować tankowania pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid vehicleFuelingId)
        {
            try
            {
                var vehicleFueling = await _context.VehicleFuelings.FirstOrDefaultAsync(item => item.Id == vehicleFuelingId);

                if (vehicleFueling is null)
                {
                    return Result.Failure("Nie znaleziono tankowania z podanym identyfikatorem!", ProblemType.NotFound);
                }

                _context.VehicleFuelings.Remove(vehicleFueling);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Tankowanie zostało pomyślnie usunięte!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete vehicle fueling with Id: {VehicleFuelingId}!", vehicleFuelingId);
                return Result.Failure("Nie można usunąć tankowania pojazdu!", ProblemType.InternalServerError);
            }
        }

        // Helpers
        private static IQueryable<VehicleFueling> ApplySortingForVehicleFueling(IQueryable<VehicleFueling> query, PaginationRequestDto paginationDto)
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

                "vehiclename" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Vehicle.Brand).ThenByDescending(x => x.Vehicle.Model)
                    : query.OrderBy(x => x.Vehicle.Brand).ThenBy(x => x.Vehicle.Model),

                _ => query.OrderByDescending(x => x.Date),
            };
        }

        private static IQueryable<VehicleFueling> ApplyFiltrationForVehicleFueling(IQueryable<VehicleFueling> query, VehicleFuelingFilterDto filterDto)
        {
            if (filterDto.VehicleId.HasValue)
            {
                query = query.Where(item => item.VehicleId == filterDto.VehicleId.Value);
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
