namespace API.Interface
{
    public interface ISignedUrlService
    {
        string GenerateSignedUrl(string cloudflareStorageKey);
    }

}
