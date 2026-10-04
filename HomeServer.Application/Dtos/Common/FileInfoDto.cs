namespace HomeServer.Application.Dtos.Common
{
    public class FileInfoDto
    {
        public string RelativePath { get; set; } = default!;
        public string ContentType { get; set; } = default!;
    }
}
