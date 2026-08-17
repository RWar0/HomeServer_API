using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;

namespace HomeServer.Application.Interfaces.MyFiles
{
    public interface IFileService
    {
        public Task<Result<FileInfoDto>> GetImageAsync(Guid photoId);
    }
}
