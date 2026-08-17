using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Interfaces.StoredFiles;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Storage;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.StoredFiles
{
    public class StoredFilesService : BaseService, IStoredFilesService
    {
        public StoredFilesService(MyDbContext context, ILogger<StoredFilesService> logger) : base(context, logger) { }

        public async Task<Result<StoredFile>> CreateAsync(FileStorageResult fileStorageResult, Guid? userId)
        {
            var result = await _context.AddAsync(new StoredFile()
            {
                OriginalName = fileStorageResult.OriginalName,
                StoredName = fileStorageResult.StoredName,
                RelativePath = fileStorageResult.RelativePath,
                ContentType = fileStorageResult.ContentType,
                Size = fileStorageResult.Size,
                CreatedByUserId = userId,
            });

            await _context.SaveChangesAsync();

            return Result<StoredFile>.Success(result.Entity);
        }
    }
}
