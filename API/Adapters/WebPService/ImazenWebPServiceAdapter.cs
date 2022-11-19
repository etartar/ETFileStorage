using API.Services;
using Imazen.WebP;
using System.Drawing;

namespace API.Adapters.WebPService
{
    public class ImazenWebPServiceAdapter : IConvertWebPService
    {
        public ImazenWebPServiceAdapter()
        {
        }

        public string Convert(string newFilePath, string webpImagePath, string webpFileName)
        {
            using (Bitmap bitmap = new Bitmap(newFilePath))
            {
                using (var bitmapStream = File.Open(webpImagePath, FileMode.Create))
                {
                    Bitmap bm = new Bitmap(bitmap);
                    SimpleEncoder encoder = new SimpleEncoder();
                    encoder.Encode(bm, bitmapStream, 100);
                }
            }

            File.Delete(newFilePath);

            return webpFileName;
        }
    }
}
