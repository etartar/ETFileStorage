using API.Services;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace API.Adapters.BitmapService
{
    public class ImageFactoryWebPServiceAdapter : IConvertWebPService
    {
        public ImageFactoryWebPServiceAdapter()
        {
        }

        public string Convert(string newFilePath, string webpImagePath, string webpFileName)
        {
            using (FileStream webPFileStream = new FileStream(webpImagePath, FileMode.Create))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(newFilePath)
                        .Format(new WebPFormat())
                        .Quality(100)
                        .Save(webPFileStream);
                }
            }

            File.Delete(newFilePath);

            return webpFileName;
        }
    }
}
