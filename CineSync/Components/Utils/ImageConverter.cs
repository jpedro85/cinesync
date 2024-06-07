using Microsoft.AspNetCore.Components.Forms;

namespace CineSync.Components.Utils
{
    public static class ImageConverter
    {
        public static string ConverBytesTo64(byte[] img)
        {
            return "data:image/jpeg;base64," + Convert.ToBase64String(img);
        }

        public static string ConverBytesTo64(byte[] img, string contentType)
        {
            return $"data:{contentType};base64," + Convert.ToBase64String(img);
        }

        public static async Task<byte[]> ReadImageAsBase64Async(IBrowserFile file, long maxFileSize)
        {
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await file.OpenReadStream(maxFileSize).CopyToAsync(memoryStream);
                    byte[] buffer = memoryStream.ToArray();
                    return buffer;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An error occurred while reading the file.", ex);
                }
            }
        }
    }
}
