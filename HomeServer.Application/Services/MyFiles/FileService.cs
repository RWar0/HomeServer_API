using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Constraints;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.MyFiles;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HomeServer.Infrastructure.Storage;

namespace HomeServer.Application.Services.MyFiles
{
    public class FileService : BaseService, IFileService
    {
        private readonly FileStorageService _fileStorageService;

        public FileService(MyDbContext context, ILogger<FileService> logger, FileStorageService fileStorageService) : base(context, logger)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<FileInfoDto>> GetImage(Guid photoId)
        {
            try
            {
                var metaData = await _context.StoredFiles
                    .Where(item => item.Id == photoId && item.DeletedAt == null)
                    .FirstOrDefaultAsync();

                if (metaData is null)
                {
                    return Result.Failure("Nie znaleziono zdjęcia o podanym identyfikatorze!", ProblemType.NotFound);
                }

                var filePath = _fileStorageService.GetFullFilePath(metaData.RelativePath);

                if (!File.Exists(filePath))
                {
                    return Result.Failure("Nie odnaleziono pliku zdjęcia!", ProblemType.BadRequest);
                }

                if (!ImagesConstraints.ContentTypes.Contains(metaData.ContentType, StringComparer.OrdinalIgnoreCase))
                {
                    return Result.Failure("Podany plik nie jest typu graficznego!", ProblemType.BadRequest);
                }

                var extension = Path.GetExtension(filePath);
                if(extension is null)
                {
                    return Result.Failure("Nie można odczytać typu zdjęcia!", ProblemType.BadRequest);
                }

                if (!ImagesConstraints.Extensions.Contains(extension))
                {
                    return Result.Failure("Podany plik nie jest zdjęciem!", ProblemType.BadRequest);
                }

                return Result.Success(new FileInfoDto
                {
                    RelativePath = filePath,
                    ContentType = metaData.ContentType,
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get image with Id: {photoId}", photoId);
                return Result.Failure("Nie można pobrać zdjęcia o podanym identyfikatorze!", ProblemType.InternalServerError);
            }
        }
    }
}
