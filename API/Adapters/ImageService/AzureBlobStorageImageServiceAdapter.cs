using API.Configs;
using API.Extensions;
using API.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace API.Adapters.ImageService
{
    public class AzureBlobStorageImageServiceAdapter : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageImageServiceAdapter(IConfiguration configuration)
        {
            var azureBlobStorage = configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorage>();
            _blobServiceClient = new BlobServiceClient($"DefaultEndpointsProtocol=https;AccountName={azureBlobStorage.AccountName};AccountKey={azureBlobStorage.AccountKey};EndpointSuffix={azureBlobStorage.EndpointSuffix}");
        }

        public string Upload(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";

            CheckExtension(extensions, extension);

            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(directory);
            blobContainerClient.CreateIfNotExists();
            blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);

            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueName.ToString());

            using FileStream fileStream = new FileStream(fileName, FileMode.Create);
            blobClient.Upload(fileStream);

            return fileName;
        }

        /// <summary>
        /// GetBlobContainerClient(blobContainerName) -> directory
        /// GetBlobClient(blobName) -> fileName
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="optionalParameters"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";

            CheckExtension(extensions, extension);

            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(directory);
            await blobContainerClient.CreateIfNotExistsAsync();
            await blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueName.ToString());

            using FileStream fileStream = new FileStream(fileName, FileMode.Create);
            await blobClient.UploadAsync(fileStream);

            return fileName;
        }

        /// <summary>
        /// GetBlobContainerClient(blobContainerName) -> directory
        /// parameters[0] -> directory,
        /// parameters[1] -> fileName
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="directory"></param>
        public void Delete(params string[] parameters)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(parameters[0]);
            BlobClient blobClient = blobContainerClient.GetBlobClient(parameters[1]);
            blobClient.DeleteIfExists();
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
