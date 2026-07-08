using HomeServer.Application.Common.Results;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Storage;

namespace HomeServer.Application.Interfaces.StoredFiles
{
    public interface IStoredFilesService
    {
        public Task<Result<StoredFile>> Create(FileStorageResult fileStorageResult, Guid? userId);
    }
}
