using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class StoredFile : BaseModel
    {
        public string OriginalName { get; set; } = default!;

        public string StoredName { get; set; } = default!;

        public string RelativePath { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public long Size { get; set; }


        public Guid? CreatedByUserId { get; set; } = null;
    }
}
