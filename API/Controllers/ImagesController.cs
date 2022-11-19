using API.Constants;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            Dictionary<string, object> optionalParameters = new();
            optionalParameters.Add(ImageServiceConstants.DIRECTORY, "Images");
            optionalParameters.Add(ImageServiceConstants.CONVERTWEBP, true);
            optionalParameters.Add(ImageServiceConstants.EXTENSIONS, ".jpg;.jpeg;.png");

            string getFile = await _imageService.UploadAsync(file, optionalParameters);

            return Ok(getFile);
        }

        [HttpDelete]
        public IActionResult Delete(string fileName)
        {
            _imageService.Delete("Images", fileName);

            return NoContent();
        }
    }
}
