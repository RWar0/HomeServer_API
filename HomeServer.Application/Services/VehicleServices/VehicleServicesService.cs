using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.VehicleServiceItems;
using HomeServer.Application.Dtos.VehicleServices;
using HomeServer.Application.Interfaces.Calendar;
using HomeServer.Application.Interfaces.VehicleServices;
using HomeServer.Domain.Enums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.VehicleServices
{
    public class VehicleServicesService : BaseService, IVehicleServicesService, ICalendarEventProvider
    {
        public VehicleServicesService(MyDbContext context, ILogger<VehicleServicesService> logger) : base(context, logger)
        { }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateVehicleServiceDto createDto)
        {
            try
            {
                var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == createDto.VehicleId);

                if (!vehicleExists)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var vehicleService = CreateVehicleService(createDto.VehicleId, createDto.Title, createDto.Date, createDto.Cost, createDto.Mileage, createDto.Items);

                await _context.VehicleServices.AddAsync(vehicleService);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Serwis pojazdu został poprawnie utworzony!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create vehicle service!");
                return Result.Failure("Nie można utworzyć serwisu pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> CreateForVehicleAsync(Guid vehicleId, CreateVehicleServiceForVehicleDto createDto)
        {
            try
            {
                var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == vehicleId);

                if (!vehicleExists)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var vehicleService = CreateVehicleService(vehicleId, createDto.Title, createDto.Date, createDto.Cost, createDto.Mileage, createDto.Items);

                await _context.VehicleServices.AddAsync(vehicleService);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Serwis pojazdu został poprawnie utworzony!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create vehicle service!");
                return Result.Failure("Nie można utworzyć serwisu pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<VehicleServiceListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto, VehicleServiceFilterDto filterDto)
        {
            try
            {
                var baseQuery = _context.VehicleServices.AsQueryable();

                baseQuery = ApplyFiltrationForVehicleService(baseQuery, filterDto);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForVehicleService(baseQuery, paginationDto);

                var services = await baseQuery
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new VehicleServiceListItemDto
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Cost = item.Cost,
                        Date = item.Date,
                        Mileage = item.Mileage,
                        VehicleId = item.VehicleId,
                        VehicleName = $"{item.Vehicle.Brand} {item.Vehicle.Model}",
                        ItemsCount = item.Items.Count()
                    })
                    .ToListAsync();

                return Result<PagedResult<VehicleServiceListItemDto>>.Success(new()
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
                _logger.LogError(ex, "Cannot fetch vehicle services!");
                return Result.Failure("Nie można załadować serwisów pojazdów!", ProblemType.InternalServerError);
            }
        }

        public async Task<IEnumerable<CalendarEventDto>> GetCalendarEventsAsync(DateFilterDto filterDto, CancellationToken ct)
        {
            var items = await _context.VehicleServices
                .Where(item => item.Date >= filterDto.FromDate && item.Date <= filterDto.ToDate)
                .Select(item => new CalendarEventDto
                {
                    Id = item.Id,
                    Title = "Serwis pojazdu",
                    Description = $"Serwis pojazdu: {item.Vehicle.Brand} {item.Vehicle.Model}. \n" +
                        $"Tytuł: \"{item.Title}\". \n" +
                        $"{GetCostLabelForCalendarEvent(item)} \n" +
                        $"Ilość pozycji serwisu: {item.Items.Count()}.",
                    Location = $"{item.Vehicle.Brand} {item.Vehicle.Model}",
                    Date = item.Date,
                    Time = null,
                    Category = CalendarEventCategoryEnum.Vehicle.ToString(),
                    IsCustomEvent = false
                })
                .ToListAsync(ct);

            return items;
        }


        public async Task<Result<VehicleServiceEditDto>> GetForEditAsync(Guid vehicleServiceId)
        {
            try
            {
                var vehicleService = await _context.VehicleServices
                    .Where(item => item.Id == vehicleServiceId)
                    .Select(item => new VehicleServiceEditDto
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Cost = item.Cost,
                        Mileage = item.Mileage,
                        Date = item.Date,
                        VehicleId = item.VehicleId,
                        Items = item.Items.Select(vi => new VehicleServiceItemDto
                        {
                            Id = vi.Id,
                            Title = vi.Title,
                            Cost = vi.Cost,
                            Description = vi.Description,
                        })
                            .OrderBy(vi => vi.Title)
                            .ToList(),
                    })
                    .FirstOrDefaultAsync();

                if (vehicleService is null)
                {
                    return Result.Failure("Nie znaleziono serwisu pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(vehicleService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get vehicle service for edit with Id: {VehicleServiceId}", vehicleServiceId);
                return Result.Failure("Nie można pobrać serwisu pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<VehicleServiceForVehicleEditDto>> GetForEditWithoutVehicleAsync(Guid vehicleServiceId)
        {
            try
            {
                var vehicleService = await _context.VehicleServices
                    .Where(item => item.Id == vehicleServiceId)
                    .Select(item => new VehicleServiceForVehicleEditDto
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Cost = item.Cost,
                        Date = item.Date,
                        Mileage = item.Mileage,
                        Items = item.Items.Select(vi => new VehicleServiceItemDto
                        {
                            Id = vi.Id,
                            Title = vi.Title,
                            Cost = vi.Cost,
                            Description = vi.Description,
                        })
                            .OrderBy(vi => vi.Title)
                            .ToList(),
                    })
                    .FirstOrDefaultAsync();

                if (vehicleService is null)
                {
                    return Result.Failure("Nie znaleziono serwisu pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(vehicleService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get vehicle service for edit without vehicle with Id: {VehicleServiceId}", vehicleServiceId);
                return Result.Failure("Nie można pobrać serwisu pojazdu z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid vehicleServiceId, UpdateVehicleServiceDto updateDto)
        {
            try
            {
                var vehicleService = await _context.VehicleServices
                    .Include(item => item.Items)
                    .FirstOrDefaultAsync(v => v.Id == vehicleServiceId);

                if (vehicleService is null)
                {
                    return Result.Failure("Nie znaleziono serwisu pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == updateDto.VehicleId);

                if (!vehicleExists)
                {
                    return Result.Failure("Nie znaleziono pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                vehicleService.Title = updateDto.Title;
                vehicleService.Cost = updateDto.Cost;
                vehicleService.Mileage = updateDto.Mileage;
                vehicleService.Date = updateDto.Date;
                vehicleService.VehicleId = updateDto.VehicleId;

                var syncResult = SyncItems(vehicleService, updateDto.Items);

                if (syncResult.IsFailure)
                {
                    return (Result<MessageResponseDto>)syncResult;
                }

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Serwis pojazdu został poprawnie zaktualizowany!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update vehicle service with Id: {VehicleServiceId}", vehicleServiceId);
                return Result.Failure("Nie można zaktualizować serwisu pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateForVehicleAsync(Guid vehicleServiceId, UpdateVehicleServiceForVehicleDto updateDto)
        {
            try
            {
                var vehicleService = await _context.VehicleServices
                    .Include(item => item.Items)
                    .FirstOrDefaultAsync(v => v.Id == vehicleServiceId);

                if (vehicleService is null)
                {
                    return Result.Failure("Nie znaleziono serwisu pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                vehicleService.Title = updateDto.Title;
                vehicleService.Cost = updateDto.Cost;
                vehicleService.Mileage = updateDto.Mileage;
                vehicleService.Date = updateDto.Date;

                var syncResult = SyncItems(vehicleService, updateDto.Items);

                if (syncResult.IsFailure)
                {
                    return (Result<MessageResponseDto>)syncResult;
                }

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Serwis pojazdu został poprawnie zaktualizowany!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update vehicle service for vehicle with Id: {VehicleServiceId}", vehicleServiceId);
                return Result.Failure("Nie można zaktualizować serwisu pojazdu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid vehicleServiceId)
        {
            try
            {
                var vehicleService = await _context.VehicleServices
                    .Include(item => item.Items)
                    .FirstOrDefaultAsync(v => v.Id == vehicleServiceId);

                if (vehicleService is null)
                {
                    return Result.Failure("Nie znaleziono serwisu pojazdu z podanym identyfikatorem!", ProblemType.NotFound);
                }

                foreach (var item in vehicleService.Items.ToList())
                {
                    item.DeletedAt = DateTime.UtcNow;
                }

                _context.VehicleServices.Remove(vehicleService);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = "Serwis pojazdu został poprawnie usunięty!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete vehicle service with Id: {VehicleServiceId}", vehicleServiceId);
                return Result.Failure("Nie można usunąć serwisu pojazdu!", ProblemType.InternalServerError);
            }
        }

        // Helpers
        private static VehicleService CreateVehicleService(Guid vehicleId, string title, DateOnly date, decimal? cost, int? mileage, IEnumerable<CreateVehicleServiceItemDto> items)
        {
            return new VehicleService
            {
                Title = title,
                Cost = cost,
                Mileage = mileage,
                Date = date,
                VehicleId = vehicleId,
                Items = [.. items.Select(item => new VehicleServiceItem
                {
                    Title = item.Title,
                    Cost = item.Cost,
                    Description = item.Description
                })]
            };
        }

        /// <summary>
        /// Synchronizes vehicle service items with the provided DTOs (removes, updates, adds).
        /// </summary>
        /// <param name="vehicleService">Entity from DB to sync items</param>
        /// <param name="itemDtos">DTOs to synchronize</param>
        /// <returns>Result indicating whether synchronization succeeded or failed</returns>
        private static Result SyncItems(VehicleService vehicleService, List<UpdateVehicleServiceItemDto> itemDtos)
        {
            var itemIds = itemDtos
                    .Where(x => x.Id.HasValue)
                    .Select(x => x.Id!.Value)
                    .ToHashSet();

            if (itemIds.Count != itemDtos.Count(x => x.Id.HasValue))
            {
                return Result.Failure("Pozycja serwisu została powielona w trakcie aktualizacji!", ProblemType.BadRequest);
            }

            var existingItems = vehicleService.Items.ToList();

            // Delete from Db items, which are not in the updateDto.Items list (removed).
            foreach (var existingItem in existingItems)
            {
                if (!itemIds.Contains(existingItem.Id))
                {
                    vehicleService.Items.Remove(existingItem);
                }
            }

            // Process new or existing vehicle service items
            foreach (var itemDto in itemDtos)
            {
                // Update existing item if it exists
                if (itemDto.Id.HasValue)
                {
                    var item = vehicleService.Items.FirstOrDefault(x => x.Id == itemDto.Id.Value);

                    if (item is null)
                    {
                        return Result.Failure("Nie znaleziono podanej pozycji serwisu pojazdu!", ProblemType.NotFound);
                    }

                    item.Title = itemDto.Title;
                    item.Description = itemDto.Description;
                    item.Cost = itemDto.Cost;
                }
                // Add new item if it doesn't exist yet.
                else
                {
                    vehicleService.Items.Add(new()
                    {
                        Title = itemDto.Title,
                        Description = itemDto.Description,
                        Cost = itemDto.Cost
                    });
                }
            }

            return Result.Success();
        }

        private static IQueryable<VehicleService> ApplySortingForVehicleService(IQueryable<VehicleService> query, PaginationRequestDto paginationDto)
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

        private static IQueryable<VehicleService> ApplyFiltrationForVehicleService(IQueryable<VehicleService> query, VehicleServiceFilterDto filterDto)
        {
            if (!string.IsNullOrWhiteSpace(filterDto.Title))
            {
                query = query.Where(v => EF.Functions.ILike(v.Title, $"%{filterDto.Title}%"));
            }

            if (filterDto.VehicleId.HasValue)
            {
                query = query.Where(v => v.VehicleId == filterDto.VehicleId.Value);
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

        /// <summary>
        /// Method returns the cost of the vehicle service, either from the Cost property or calculated from the sum of the costs of its items.
        /// If any of the items does not have a cost, it returns null.
        /// </summary>
        /// <param name="vehicleService">Vehicle service for which to calculate the cost</param>
        /// <returns>The cost of the vehicle service or null if any of the items cannot be determined</returns>
        private static decimal? GetExactEnteredOrCalculatedCost(VehicleService vehicleService)
        {
            if (vehicleService.Cost.HasValue)
            {
                return vehicleService.Cost.Value;
            }

            if (vehicleService.Items.Count > 0)
            {
                decimal sum = 0;

                foreach (var item in vehicleService.Items)
                {
                    if (!item.Cost.HasValue)
                    {
                        return null;
                    }

                    sum += item.Cost.Value;
                }
            }

            return null;
        }

        private static string GetCostLabelForCalendarEvent(VehicleService vehicleService)
        {
            var cost = GetExactEnteredOrCalculatedCost(vehicleService);
            return cost.HasValue ? $"Koszt: {cost.Value:C}." : "Koszt nie został oreślony.";
        }
    }
}
