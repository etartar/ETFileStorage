using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using API.Configs;
using API.Extensions;
using API.Services;

namespace API.Adapters.ImageService
{
    public class AmazonS3ImageServiceAdapter : IImageService
    {
        private readonly AWSS3Configuration _awsConfig;

        public AmazonS3ImageServiceAdapter(IConfiguration configuration)
        {
            _awsConfig = configuration.GetSection("AWSS3Configuration").Get<AWSS3Configuration>();
        }

        public string Upload(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";

            CheckExtension(extensions, extension);

            BasicAWSCredentials credentials = new BasicAWSCredentials(_awsConfig.AccessKey, _awsConfig.SecretKey);
            AmazonS3Config config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsConfig.Region)
            };

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = formFile.OpenReadStream(),
                Key = fileName,                
                BucketName = directory,
                CannedACL = S3CannedACL.NoACL
            };

            using (var client = new AmazonS3Client(credentials, config))
            {
                var transferUtility = new TransferUtility(client);
                transferUtility.Upload(uploadRequest);
            }

            return fileName;
        }

        public async Task<string> UploadAsync(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";

            CheckExtension(extensions, extension);

            BasicAWSCredentials credentials = new BasicAWSCredentials(_awsConfig.AccessKey, _awsConfig.SecretKey);
            AmazonS3Config config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsConfig.Region)
            };

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = formFile.OpenReadStream(),
                Key = fileName,
                BucketName = directory,
                CannedACL = S3CannedACL.NoACL
            };

            using (var client = new AmazonS3Client(credentials, config))
            {
                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);
            }

            return fileName;
        }

        /// <summary>
        /// parameters[0] -> bucketname (directory),
        /// parameters[1] -> fileName
        /// </summary>
        public void Delete(params string[] parameters)
        {
            BasicAWSCredentials credentials = new BasicAWSCredentials(_awsConfig.AccessKey, _awsConfig.SecretKey);
            AmazonS3Config config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsConfig.Region)
            };

            using (var client = new AmazonS3Client(credentials, config))
            {
                client.DeleteObjectAsync(parameters[0], parameters[1]);
            }
        }

        #region Private Methods

        private void CheckExtension(string[] extensions, string extension)
        {
            if (!extensions.Contains(extension))
                throw new NotSupportedException($"Extension ({extension}) cannot supported.");
        }

        #endregion
    }
}
