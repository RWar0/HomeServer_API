using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Common.Select;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.WaterChanges;
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
                    .Select(item => new AquariumGetResponseDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                        LastPhotoId = item.Photos
                                            .OrderByDescending(photo => photo.CreatedAt)
                                            .Select(photo => (Guid?)photo.Id)
                                            .FirstOrDefault(),
                        LastWaterChange = item.WaterChanges
                            .OrderByDescending(waterChange => waterChange.ChangeDate)
                            .Select(waterChange => (DateOnly?)waterChange.ChangeDate)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Result.Success(items.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get list of all aquariums");
                return Result.Failure("Nie można pobrać akwariów z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<SelectResponseDto[]>> GetForSelect()
        {
            try
            {
                var items = await _context.Aquariums
                    .Select(item => new SelectResponseDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                    })
                    .ToListAsync();

                return Result.Success(items.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get list of all aquariums");
                return Result.Failure("Nie można pobrać akwariów z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<AquariumDetailsDto>> GetDetails(Guid aquariumId)
        {
            try
            {
                var item = await _context.Aquariums
                    .Where(item => item.Id == aquariumId)
                    .Select(item => new AquariumDetailsDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                        PhotosCount = item.Photos.Count(),
                        LastPhotoId = item.Photos
                                            .OrderByDescending(photo => photo.CreatedAt)
                                            .Select(photo => (Guid?)photo.Id)
                                            .FirstOrDefault(),
                        LastWaterChange = item.WaterChanges
                            .OrderByDescending(waterChange => waterChange.ChangeDate)
                            .Select(waterChange => (DateOnly?)waterChange.ChangeDate)
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

        public async Task<Result<PagedResult<AquariumPhotosResponseDto>>> GetPhotos(Guid aquariumId, PaginationDto paginationDto)
        {
            try
            {
                var photosQuery = _context.Aquariums
                    .Where(item => item.Id == aquariumId)
                    .SelectMany(item => item.Photos);

                var totalQuantity = await photosQuery.CountAsync();

                var result = await photosQuery
                    .OrderByDescending(item => item.CreatedAt)
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(photo => new AquariumPhotosResponseDto
                    {
                        Id = photo.Id,
                        OriginalName = photo.OriginalName,
                        CreatedAt = photo.CreatedAt,
                        CreatedBy = _context.Users
                            .IgnoreQueryFilters()
                            .Where(user => user.Id == photo.CreatedByUserId)
                            .Select(user => user.Name)
                            .FirstOrDefault() ?? "Brak informacji"
                    })
                    .ToListAsync();

                return Result.Success(new PagedResult<AquariumPhotosResponseDto>
                {
                    Data = result,
                    Pagination = new()
                    {
                        Page = paginationDto.Page,
                        PageSize = paginationDto.PageSize,
                        TotalCount = totalQuantity,
                        SortBy = paginationDto.SortBy,
                        SortDirection = paginationDto.SortDirection,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get photos of aquariums aquarium with Id: {aquariumId}", aquariumId);
                return Result.Failure("Można pobrać zdjęć akwarium z podanym identyfikatorem!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<WaterChangesOfAquariumGetResponseDto>>> GetWaterChanges(Guid aquariumId, PaginationDto paginationDto)
        {
            try
            {
                var isAquariumExists = await _context.Aquariums.AnyAsync(item => item.Id == aquariumId);

                if (!isAquariumExists)
                {
                    return Result.Failure("Akwarium o podanym identyfikatorze nie istnieje!", ProblemType.NotFound);
                }

                var baseQuery = _context.WaterChanges.AsNoTracking().Where(item => item.Aquarium.Id == aquariumId);

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForWaterChange(baseQuery, paginationDto);

                var waterChanges = await baseQuery
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new WaterChangesOfAquariumGetResponseDto()
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        ChangeDate = item.ChangeDate,
                    })
                    .ToListAsync();

                return Result<PagedResult<WaterChangesOfAquariumGetResponseDto>>.Success(new()
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
                _logger.LogError(ex, "Cannot get list of aquarium with Id: {AquariumId} water changes", aquariumId);
                return Result.Failure("Nie można pobrać listy podmian wody podanego akwarium z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<AquariumBaseDataDto>> GetForEdit(Guid aquariumId)
        {
            try
            {
                var aquarium = await _context.Aquariums
                    .Where(item => item.Id == aquariumId)
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
                var aquarium = await _context.Aquariums.FirstOrDefaultAsync(item => item.Id == aquariumId);

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
                var aquarium = await _context.Aquariums.Include(item => item.Photos).Where(item => item.Id == aquariumId).FirstOrDefaultAsync();

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

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Akwarium \"{aquarium.Name}\" edytowano pomyślnie!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update aquarium with Id: {AquariumId}", aquariumId);
                return Result.Failure("Nie można zaktualizować akwarium!", ProblemType.InternalServerError);
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

                _context.Remove(aquarium);

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

        public async Task<Result<MessageResponseDto>> DeletePhoto(Guid aquariumId, Guid photoId)
        {
            try
            {
                var photo = await _context.Aquariums
                    .Where(aquarium => aquarium.Id == aquariumId)
                    .SelectMany(aquarium => aquarium.Photos)
                    .FirstOrDefaultAsync(photo => photo.Id == photoId);

                if (photo is null)
                {
                    return Result.Failure("Nie znaleziono zdjęcia o podanym identyfikatorze!", ProblemType.NotFound);
                }

                _context.Remove(photo);

                await _context.SaveChangesAsync();

                await _fileStorageService.DeleteAsync(photo.RelativePath);

                return Result.Success(new MessageResponseDto()
                {
                    Message = "Zdjęcie zostało poprawnie usunięte z akwarium"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete photo with Id: {PhotoId}, from aquarium with Id: {AquariumId}", photoId, aquariumId);
                return Result.Failure("Nie można usunąć zdjęcia akwarium!", ProblemType.InternalServerError);
            }
        }

        // Helpers
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

                _ => query.OrderByDescending(x => x.ChangeDate)
            };
        }
    }
}
