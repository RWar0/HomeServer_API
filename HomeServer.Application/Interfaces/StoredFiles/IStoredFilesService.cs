using HomeServer.Application.Common.Results;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Storage;

namespace HomeServer.Application.Interfaces.StoredFiles
{
    public interface IStoredFilesService
    {
        public Task<Result<StoredFile>> CreateAsync(FileStorageResult fileStorageResult, Guid? userId);
    }
}
