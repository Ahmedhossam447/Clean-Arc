namespace CleanArc.Core.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string filename);
        Task DeleteImageAsync(string fileUrl);
    }
}
