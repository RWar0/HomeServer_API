using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Aquariums
{
    public class AquariumsService : BaseService, IAquariumsService
    {
        public AquariumsService(MyDbContext context, ILogger<AquariumsService> logger) : base(context, logger) { }

        public async Task<Result<MessageResponseDto>> Create(AquariumCreateDto createDto)
        {
            try
            {
                await _context.AddAsync(new Aquarium()
                {
                    Name = createDto.Name,
                    Volume = createDto.Volume,
                    CreationDate = createDto.CreationDate
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto()
                {
                    Message = $"Akwarium \"{createDto.Name}\" zostało utworzone!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create aquarium {AquariumName}", createDto.Name);
                return Result.Failure("Nie dodano akwarium do bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<AquariumGetResponseDto[]>> GetAll()
        {
            try
            {
                var items = await _context.Aquariums
                    .Include(item => item.Photos)
                    .Where(item => item.DeletedAt == null)
                    .Select(item => new AquariumGetResponseDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                        LastPhotoId = item.Photos
                                            .OrderByDescending(item => item.CreatedAt)
                                            .First().Id
                    })
                    .ToListAsync();

                return Result.Success(items.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get list of all aquariums");
                return Result.Failure("Można pobrać akwariów z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<AquariumBaseDataDto>> GetForEdit(Guid aquariumId)
        {
            try
            {
                var aquarium = await _context.Aquariums
                    .Where(item =>
                        item.Id == aquariumId &&
                        item.DeletedAt == null)
                    .Select(item => new AquariumBaseDataDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                    })
                    .FirstOrDefaultAsync();

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium o podanym identyfikatorze!!", ProblemType.NotFound);
                }

                return Result.Success(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get aquarium with Id: {aquariumId}", aquariumId);
                return Result.Failure("Można pobrać akwarium z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> Update(Guid aquariumId, AquariumCreateDto editDto)
        {
            try
            {
                var aquarium = await _context.Aquariums
                    .Where(item =>
                        item.Id == aquariumId &&
                        item.DeletedAt == null)
                    .FirstOrDefaultAsync();

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium o podanym identyfikatorze!!", ProblemType.NotFound);
                }

                aquarium.Name = editDto.Name;
                aquarium.Volume = editDto.Volume;
                aquarium.CreationDate = editDto.CreationDate;

                aquarium.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto()
                {
                    Message = $"Akwarium \"{aquarium.Name}\" edytowano pomyślnie!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update aquarium with Id: {aquariumId}", aquariumId);
                return Result.Failure("Można zaktualizować akwarium!", ProblemType.InternalServerError);
            }
        }
    }
}
