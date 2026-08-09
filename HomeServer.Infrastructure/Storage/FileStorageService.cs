namespace HomeServer.Infrastructure.Storage
{
    public class FileStorageService
    {

        public FileStorageService() { }

        public async Task<FileStorageResult> SaveAsync(Stream stream, string fileName, string contentType, string? subDirectory = null)
        {
            var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage");
            var safeRootPath = Path.GetFullPath(rootPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            var targetDirectory = subDirectory is null
                ? safeRootPath
                : Path.GetFullPath(Path.Combine(safeRootPath, subDirectory));

            if (!targetDirectory.StartsWith(safeRootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Invalid storage path.");
            }

            Directory.CreateDirectory(targetDirectory);

            var extension = Path.GetExtension(fileName);
            var storedName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(targetDirectory, storedName);

            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            var relativePath = subDirectory is null
                ? Path.Combine("Storage", storedName)
                : Path.Combine("Storage", subDirectory, storedName);

            return new FileStorageResult
            {
                OriginalName = fileName,
                StoredName = storedName,
                RelativePath = relativePath,
                ContentType = contentType,
                Size = fileStream.Length
            };
        }

        public string GetFullFilePath(string relativePath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        public bool IsFileExists(string relativePath)
        {
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            return File.Exists(fullPath);
        }

        public string GetExtension(string fullPath) => Path.GetExtension(fullPath);

        public async Task DeleteAsync(string relativePath)
        {
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            await Task.CompletedTask;
        }
    }
}
