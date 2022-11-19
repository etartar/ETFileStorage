using API.Services;

namespace API.Adapters.ImageService
{
    public class AmazonS3ImageServiceAdapter : IImageService
    {
        public AmazonS3ImageServiceAdapter()
        {
        }

        public string Upload(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadAsync(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            throw new NotImplementedException();
        }

        public void Delete(params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
