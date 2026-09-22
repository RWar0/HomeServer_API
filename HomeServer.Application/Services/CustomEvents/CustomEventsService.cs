using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.CustomEvents;
using HomeServer.Application.Interfaces.CustomEvents;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.CustomEvents
{
    public class CustomEventsService : BaseService, ICustomEventsService
    {
        public CustomEventsService(MyDbContext context, ILogger<CustomEventsService> logger) : base(context, logger) { }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateCustomEventDto createDto, Guid createdByUserId)
        {
            try
            {
                await _context.CustomEvents.AddAsync(new()
                {
                    Title = createDto.Title,
                    Description = createDto.Description,
                    Location = createDto.Location,
                    Category = createDto.Category,
                    Date = createDto.Date,
                    Time = createDto.Time,
                    CreatedByUserId = createdByUserId
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Wydarzenie \"{createDto.Title}\" zostało poprawnie utworzone!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create custom event!");
                return Result.Failure("Nie można utworzyć wydarzenia!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<CalendarEventDto[]>> GetAsync(DateFilterDto filterDto)
        {
            try
            {
                var query = _context.CustomEvents.AsNoTracking().AsQueryable();

                query = FilterCustomEventByDate(query, filterDto);

                var customEvents = await query
                    .Select(item => new CalendarEventDto
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = item.Description,
                        Location = item.Location,
                        Category = item.Category.ToString(),
                        Date = item.Date,
                        Time = item.Time,
                        CompletionDate = item.CompletionDate,
                        IsCustomEvent = true
                    })
                    .ToListAsync();

                return Result.Success(customEvents.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get custom events!");
                return Result.Failure("Nie można pobrać wydarzeń!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<CalendarEventDto>> GetByIdAsync(Guid customEventId)
        {
            try
            {
                var customEvent = await _context.CustomEvents
                    .Where(item => item.Id == customEventId)
                    .Select(item => new CalendarEventDto
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = item.Description,
                        Location = item.Location,
                        Category = item.Category.ToString(),
                        Date = item.Date,
                        Time = item.Time,
                        CompletionDate = item.CompletionDate,
                        IsCustomEvent = true
                    })
                    .FirstOrDefaultAsync();

                if (customEvent is null)
                {
                    return Result.Failure("Nie znaleziono wydarzenia z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result.Success(customEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get event with Id: {CustomEventId}", customEventId);
                return Result.Failure("Nie można pobrać wydarzenia!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> MarkDoneNowAsync(Guid customEventId, Guid updatedByUserId)
        {
            try
            {
                var customEvent = await _context.CustomEvents
                    .Where(item => item.Id == customEventId)
                    .FirstOrDefaultAsync();

                if (customEvent is null)
                {
                    return Result.Failure("Nie znaleziono wydarzenia z podanym identyfikatorem!", ProblemType.NotFound);
                }

                if (customEvent.CompletionDate is not null)
                {
                    return Result.Failure("Wydarzenie jest już oznaczone jako wykonane!", ProblemType.BadRequest);
                }

                customEvent.CompletionDate = DateOnly.FromDateTime(DateTime.Now);
                customEvent.UpdatedByUserId = updatedByUserId;


                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Wydarzenie \"{customEvent.Title}\" zostało oznaczone jako wykonane!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot mark as done event with Id: {CustomEventId}", customEventId);
                return Result.Failure("Nie można oznaczyć wydarzenia jako wykonanego!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid customEventId, UpdateCustomEventDto updateDto, Guid updatedByUserId)
        {
            try
            {
                var customEvent = await _context.CustomEvents
                    .Where(item => item.Id == customEventId)
                    .FirstOrDefaultAsync();

                if (customEvent is null)
                {
                    return Result.Failure("Nie znaleziono wydarzenia z podanym identyfikatorem!", ProblemType.NotFound);
                }

                customEvent.Title = updateDto.Title;
                customEvent.Description = updateDto.Description;
                customEvent.Category = updateDto.Category;
                customEvent.Date = updateDto.Date;
                customEvent.Time = updateDto.Time;
                customEvent.Location = updateDto.Location;
                customEvent.UpdatedByUserId = updatedByUserId;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Wydarzenie \"{customEvent.Title}\" zostało poprawnie zaktualizowane!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update event with Id: {CustomEventId}", customEventId);
                return Result.Failure("Nie można zaktualizować wydarzenia!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid customEventId, Guid deletedByUserId)
        {
            try
            {
                var customEvent = await _context.CustomEvents
                    .Where(item => item.Id == customEventId)
                    .FirstOrDefaultAsync();

                if (customEvent is null)
                {
                    return Result.Failure("Nie znaleziono wydarzenia z podanym identyfikatorem!", ProblemType.NotFound);
                }

                customEvent.UpdatedByUserId = deletedByUserId;
                _context.CustomEvents.Remove(customEvent);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Wydarzenie \"{customEvent.Title}\" zostało poprawnie usunięte!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete event with Id: {CustomEventId}", customEventId);
                return Result.Failure("Nie można usunąć wydarzenia!", ProblemType.InternalServerError);
            }
        }

        // Helpers
        private static IQueryable<CustomEvent> FilterCustomEventByDate(IQueryable<CustomEvent> query, DateFilterDto filterDto)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var fromDate = filterDto.FromDate ?? new DateOnly(today.Year, today.Month, 1);

            var toDate = filterDto.ToDate ?? fromDate.AddMonths(1).AddDays(-1);

            return query.Where(item =>
                item.Date >= fromDate.AddDays(-7) &&
                item.Date <= toDate.AddDays(7));
        }
    }
}
