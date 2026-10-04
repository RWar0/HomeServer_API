namespace HomeServer.Application.Constraints
{
    public static class ImagesConstraints
    {
        public static readonly string[] Extensions = [".png", ".jpg", ".jpeg"];
        public static readonly string[] ContentTypes = ["image/png", "image/jpg", "image/jpeg"];
    }
}
