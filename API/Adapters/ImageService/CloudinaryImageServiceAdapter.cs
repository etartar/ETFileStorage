using API.Extensions;
using API.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace API.Adapters.ImageService
{
    public class CloudinaryImageServiceAdapter : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageServiceAdapter(IConfiguration configuration)
        {
            Account? account = configuration.GetSection("CloudinaryAccount").Get<Account>();
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public string Upload(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            bool convertWebP = optionalParameters.GetConvertWebP();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";
            string newFilePath = Path.Combine(directory, fileName);

            CheckDirectory(directory);
            CheckExtension(extensions, extension);

            using MemoryStream memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            memoryStream.Position = 0;

            ImageUploadParams uploadParams = new ImageUploadParams
            {
                File = new FileDescription(newFilePath, memoryStream),
                Folder = directory
            };

            if (convertWebP)
            {
                uploadParams.Format = "webp";
            }

            ImageUploadResult result = _cloudinary.Upload(uploadParams);

            if (result.Error != null)
            {
                throw new Exception($"Cloudinary error occured: {result.Error.Message}");
            }

            return result.SecureUrl.ToString();
        }

        public async Task<string> UploadAsync(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            bool convertWebP = optionalParameters.GetConvertWebP();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";
            string newFilePath = Path.Combine(directory, fileName);

            await CheckDirectoryAsync(directory);
            CheckExtension(extensions, extension);

            using MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            ImageUploadParams uploadParams = new ImageUploadParams
            {
                File = new FileDescription(newFilePath, memoryStream),
                Folder = directory
            };

            if (convertWebP)
            {
                uploadParams.Format = "webp";
            }

            ImageUploadResult result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception($"Cloudinary error occured: {result.Error.Message}");
            }

            return result.SecureUrl.ToString();
        }

        /// <summary>
        /// parameters[0] -> fileName
        /// </summary>
        /// <param name="parameters"></param>
        public void Delete(params string[] parameters)
        {
            string publicId = GetPublicId(parameters[0]);
            DeletionParams deletionParams = new(publicId);
            DeletionResult result = _cloudinary.Destroy(deletionParams);

            if (result.Error != null)
            {
                throw new Exception($"Cloudinary delete error occured: {result.Error.Message}");
            }
        }

        #region Private Methods

        private void CheckDirectory(string folder)
        {
            var folderResult = _cloudinary.RootFolders();
            if (!folderResult.Folders.Where(x => x.Name.Contains(folder)).Any())
            {
                var result = _cloudinary.CreateFolder(folder);
                if (result.Error != null)
                {
                    throw new Exception($"Cloudinary create folder error occured: {result.Error.Message}");
                }
            }
        }

        private async Task CheckDirectoryAsync(string folder)
        {
            var folderResult = await _cloudinary.RootFoldersAsync();
            if (!folderResult.Folders.Where(x => x.Name.Contains(folder)).Any())
            {
                var result = await _cloudinary.CreateFolderAsync(folder);
                if (result.Error != null)
                {
                    throw new Exception($"Cloudinary create folder error occured: {result.Error.Message}");
                }
            }
        }

        private void CheckExtension(string[] extensions, string extension)
        {
            if (!extensions.Contains(extension))
                throw new NotSupportedException($"Extension ({extension}) cannot supported.");
        }

        private string GetPublicId(string imageUrl)
        {
            int startIndex = imageUrl.LastIndexOf('/') + 1;
            int endIndex = imageUrl.LastIndexOf('.');
            int length = endIndex - startIndex;
            return imageUrl.Substring(startIndex, length);
        }

        #endregion
    }
}
