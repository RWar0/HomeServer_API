using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Constraints;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.MyFiles;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.MyFiles
{
    public class FileService : BaseService, IFileService
    {
        public FileService(MyDbContext context, ILogger<FileService> logger) : base(context, logger) { }

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

                //if (!ImagesConstraints.Extensions.Contains(metaData.ContentType, StringComparer.OrdinalIgnoreCase))
                //{
                //    return Result.Failure("Podany plik nie jest zdjęciem!", ProblemType.BadRequest);
                //}

                return Result.Success(new FileInfoDto
                {
                    RelativePath = metaData.RelativePath,
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
