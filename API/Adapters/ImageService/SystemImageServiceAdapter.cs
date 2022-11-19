using API.Constants;
using API.Extensions;
using API.Services;

namespace API.Adapters.ImageService
{
    public class SystemImageServiceAdapter : IImageService
    {
        private readonly IHostEnvironment _environment;
        private readonly IConvertWebPService _convertWebPService;

        public SystemImageServiceAdapter(IHostEnvironment environment, IConvertWebPService convertWebPService)
        {
            _environment = environment;
            _convertWebPService = convertWebPService;
        }

        /// <summary>
        /// Upload image file
        /// </summary>
        /// <param name="formFile">Image file</param>
        /// <param name="optionalParameters">Contains: directory, image accepted extensions, convertBitmap</param>
        /// <returns>Upload image name</returns>
        public string Upload(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            bool convertWebP = optionalParameters.GetConvertWebP();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";
            string newFilePath = Path.Combine(_environment.ContentRootPath, ImageServiceConstants.MAIN_DIRECTORY, directory, fileName);
            string webpFileName = $"{uniqueName}.webp";
            string webpImagePath = Path.Combine(_environment.ContentRootPath, ImageServiceConstants.MAIN_DIRECTORY, directory, webpFileName);

            CheckDirectory(newFilePath);
            CheckExtension(extensions, extension);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            if (convertWebP)
            {
                return _convertWebPService.Convert(newFilePath, webpImagePath, webpFileName);
            }

            return fileName;
        }

        public async Task<string> UploadAsync(IFormFile formFile, Dictionary<string, object> optionalParameters)
        {
            Guid uniqueName = Guid.NewGuid();
            bool convertWebP = optionalParameters.GetConvertWebP();
            string[] extensions = optionalParameters.GetExtensions();
            string directory = optionalParameters.GetDirectory();
            string extension = Path.GetExtension(formFile.FileName).ToLower();
            string fileName = $"{uniqueName}{extension}";
            string newFilePath = Path.Combine(_environment.ContentRootPath, ImageServiceConstants.MAIN_DIRECTORY, directory, fileName);
            string webpFileName = $"{uniqueName}.webp";
            string webpImagePath = Path.Combine(_environment.ContentRootPath, ImageServiceConstants.MAIN_DIRECTORY, directory, webpFileName);

            CheckDirectory(newFilePath);
            CheckExtension(extensions, extension);

            await using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            if (convertWebP)
            {
                return _convertWebPService.Convert(newFilePath, webpImagePath, webpFileName);
            }

            return fileName;
        }

        /// <summary>
        /// parameters[0] -> directory,
        /// parameters[1] -> fileName
        /// </summary>
        /// <param name="parameters"></param>
        public void Delete(params string[] parameters)
        {
            string filePath = Path.Combine(_environment.ContentRootPath, ImageServiceConstants.MAIN_DIRECTORY, parameters[0], parameters[1]);
            CheckFileExist(filePath);
            File.Delete(filePath);
        }

        #region Private Methods

        private void CheckDirectory(string path)
        {
            new FileInfo(path).Directory?.Create();
        }

        private void CheckExtension(string[] extensions, string extension)
        {
            if (!extensions.Contains(extension))
                throw new NotSupportedException($"Extension ({extension}) cannot supported.");
        }

        private void CheckFileExist(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Image ({filePath}) cannot be found.");
        }

        #endregion
    }
}
