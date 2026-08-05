using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Aquariums
{
    public class AquariumsService : BaseService, IAquariumsService
    {
        private readonly FileStorageService _fileStorageService;

        public AquariumsService(MyDbContext context, ILogger<AquariumsService> logger, FileStorageService fileStorageService) : base(context, logger)
        {
            _fileStorageService = fileStorageService;
        }

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

                return Result.Success(new MessageResponseDto
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
                    .Where(item => item.DeletedAt == null)
                    .Select(item => new AquariumGetResponseDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                        LastPhotoId = item.Photos
                                            .OrderByDescending(photo => photo.CreatedAt)
                                            .Select(photo => (Guid?)photo.Id)
                                            .FirstOrDefault()
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

        public async Task<Result<AquariumDetailsDto>> GetDetails(Guid aquariumId)
        {
            try
            {
                var item = await _context.Aquariums
                    .Where(item => item.Id == aquariumId && item.DeletedAt == null)
                    .Select(item => new AquariumDetailsDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                        PhotosCount = item.Photos.Where(photo => photo.DeletedAt == null).Count(),
                        LastPhotoId = item.Photos
                                            .OrderByDescending(photo => photo.CreatedAt)
                                            .Select(photo => (Guid?)photo.Id)
                                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                if (item is null)
                {
                    return Result.Failure("Nie znaleziono akwarium o podanym identyfikatorze!", ProblemType.NotFound);
                }

                return Result.Success(item);
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
                _logger.LogError(ex, "Cannot get aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Można pobrać akwarium z bazy danych!", ProblemType.InternalServerError);
            }
        }

        private async Task<Result<Aquarium>> GetAquariumWithAllData(Guid aquariumId)
        {
            try
            {
                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == aquariumId && item.DeletedAt == null);

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium o podanym identyfikatorze!", ProblemType.NotFound);
                }

                return Result.Success(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get all data for aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Można pobrać akwarium!", ProblemType.InternalServerError);
            }
        }

        private async Task<Result<Aquarium>> GetAquariumWithAllDataAndPhotos(Guid aquariumId)
        {
            try
            {
                var aquarium = await _context.Aquariums.Include(item => item.Photos).Where(item => item.Id == aquariumId && item.DeletedAt == null).FirstOrDefaultAsync();

                if (aquarium is null)
                {
                    return Result.Failure("Nie znaleziono akwarium o podanym identyfikatorze!", ProblemType.NotFound);
                }

                return Result.Success(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get all data for aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Można pobrać akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UploadPhoto(Guid aquariumId, ImageDto fileDto, Guid userId)
        {
            // Pobierz akwarium wraz ze zdjęciami
            var aquariumResult = await GetAquariumWithAllDataAndPhotos(aquariumId);

            if (aquariumResult.IsFailure)
            {
                return aquariumResult.ToFailure<MessageResponseDto>();
            }

            var aquarium = aquariumResult.Value;

            // Utwórz plik obrazu na dysku
            var uploadResult = await _fileStorageService.SaveAsync(
                fileDto.File.OpenReadStream(),
                fileDto.File.FileName,
                fileDto.File.ContentType,
                Path.Join("aquariums", aquarium.Id.ToString())
            );

            // Dodaj meta-dane pliku do bazy danych
            try
            {
                StoredFile storedFile = new()
                {
                    OriginalName = uploadResult.OriginalName,
                    StoredName = uploadResult.StoredName,
                    RelativePath = uploadResult.RelativePath,
                    ContentType = uploadResult.ContentType,
                    Size = uploadResult.Size,
                    CreatedByUserId = userId,
                };

                // Dodaj plik do akwarium i do DbSet
                aquarium.Photos.Add(storedFile);
                _context.StoredFiles.Add(storedFile);

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto { Message = "Zdjęcie zostało pomyślnie przesłane" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot upload photo for aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Nie można przesłać zdjęcia!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> Update(Guid aquariumId, AquariumCreateDto editDto)
        {
            try
            {
                var aquariumResult = await GetAquariumWithAllData(aquariumId);

                if (aquariumResult.IsFailure)
                {
                    return aquariumResult.ToFailure<MessageResponseDto>();
                }

                var aquarium = aquariumResult.Value;

                aquarium.Name = editDto.Name;
                aquarium.Volume = editDto.Volume;
                aquarium.CreationDate = editDto.CreationDate;

                aquarium.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Akwarium \"{aquarium.Name}\" edytowano pomyślnie!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Można zaktualizować akwarium!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> Delete(Guid aquariumId)
        {
            try
            {
                var aquariumResult = await GetAquariumWithAllData(aquariumId);

                if (aquariumResult.IsFailure)
                {
                    return aquariumResult.ToFailure<MessageResponseDto>();
                }

                var aquarium = aquariumResult.Value;

                aquarium.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Akwarium \"{aquarium.Name}\" zostało pomyślnie usunięte!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Nie można usunąć akwarium!", ProblemType.InternalServerError);
            }
        }
    }
}
