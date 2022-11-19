namespace API.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Upload image file.
        /// Optional parameters contains: directory, image accepted extensions, convertWebP, etc.
        /// </summary>
        /// <param name="formFile">Image file</param>
        /// <param name="optionalParameters">Contains: directory, image accepted extensions, convertWebP</param>
        /// <returns>Upload image name</returns>
        string Upload(IFormFile formFile, Dictionary<string, object> optionalParameters);

        /// <summary>
        /// Upload image file.
        /// Optional parameters contains: directory, image accepted extensions, convertWebP, etc.
        /// </summary>
        /// <param name="formFile">Image file</param>
        /// <param name="optionalParameters">Contains: directory, image accepted extensions, convertWebP</param>
        /// <returns>Upload image name</returns>
        Task<string> UploadAsync(IFormFile formFile, Dictionary<string, object> optionalParameters);
        void Delete(params string[] parameters);
    }
}
