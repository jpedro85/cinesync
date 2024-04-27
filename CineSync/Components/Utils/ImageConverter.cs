namespace CineSync.Components.Utils
{
    public static class ImageConverter
    {
        public static string ConverBytesTo64(byte[] img)
        {
            return "data:image/jpeg;base64"+Convert.ToBase64String(img);
        }
    }
}
