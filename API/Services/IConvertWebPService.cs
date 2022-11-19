namespace API.Services
{
    public interface IConvertWebPService
    {
        string Convert(string newFilePath, string webpImagePath, string webpFileName);
    }
}
