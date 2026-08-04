namespace HomeServer.Application.Constraints
{
    public static class ImagesConstraints
    {
        public static readonly string[] Extensions = [".png", ".jpg", ".jpeg"];
        public static readonly string[] ContentTypes = ["images/png", "images/jpg", "images/jpeg"];
    }
}
