namespace HomeServer.Infrastructure.Storage
{
    public class FileStorageResult
    {
        public string OriginalName { get; set; } = default!;
        public string StoredName { get; set; } = default!;
        public string RelativePath { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long Size { get; set; }
    }
}
