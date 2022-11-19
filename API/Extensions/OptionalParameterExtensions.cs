using API.Constants;

namespace API.Extensions
{
    public static class OptionalParameterExtensions
    {
        public static string GetDirectory(this Dictionary<string, object> optionalParameters)
        {
            optionalParameters.TryGetValue(ImageServiceConstants.DIRECTORY, out object directory);

            if (directory is null) throw new ArgumentNullException("Directory cannot be null.");

            return Convert.ToString(directory);
        }

        public static string[] GetExtensions(this Dictionary<string, object> optionalParameters)
        {
            optionalParameters.TryGetValue(ImageServiceConstants.EXTENSIONS, out object extensions);

            if (extensions is null) throw new ArgumentNullException("Extensions cannot be null.");

            return Convert.ToString(extensions).Split(";");
        }

        public static bool GetConvertWebP(this Dictionary<string, object> optionalParameters)
        {
            optionalParameters.TryGetValue(ImageServiceConstants.CONVERTWEBP, out object convertWebP);

            return Convert.ToBoolean(convertWebP);
        }
    }
}
