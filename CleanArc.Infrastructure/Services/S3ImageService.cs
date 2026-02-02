using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CleanArc.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace CleanArc.Infrastructure.Services
{ 
    public class S3ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<S3ImageService> _logger;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _region;
        private readonly string _bucketName;
        private readonly int MaxWidth;
        private readonly int MaxHeight;
        private readonly int Quality;
        public S3ImageService(IConfiguration configuration, ILogger<S3ImageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            _accessKey = _configuration["AWS:AccessKey"] 
                ?? throw new InvalidOperationException("AWS AccessKey not configured");
            
            _secretKey = _configuration["AWS:SecretKey"] 
                ?? throw new InvalidOperationException("AWS SecretKey not configured");
            
            _region = _configuration["AWS:Region"] 
                ?? throw new InvalidOperationException("AWS Region not configured");
            
            _bucketName = _configuration["AWS:BucketName"] 
                ?? throw new InvalidOperationException("AWS BucketName not configured");
            
            MaxWidth = int.Parse(_configuration["AWS:ImageCompression:MaxWidth"] ?? "1920");
            MaxHeight = int.Parse(_configuration["AWS:ImageCompression:MaxHeight"] ?? "1080");
            Quality = int.Parse(_configuration["AWS:ImageCompression:Quality"] ?? "85");
        }

        public async Task<bool> DeleteImageAsync(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var key = uri.AbsolutePath.TrimStart('/');
                var client = new AmazonS3Client(_accessKey, _secretKey, RegionEndpoint.GetBySystemName(_region));
                
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };
                
                var response = await client.DeleteObjectAsync(deleteRequest);
                
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent || 
                    response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("Image deleted successfully: {Key}", key);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from S3: {FileUrl}", fileUrl);
                return false;
            }
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string filename)
        {
            _logger.LogInformation("Starting image upload: {FileName}", filename);
            
            var name = $"{Guid.NewGuid()}_{filename}";
            var compressedImage = await CompressImageAsync(imageStream, filename);
            var contentType = GetContentType(filename);
            var client = new AmazonS3Client(_accessKey, _secretKey, RegionEndpoint.GetBySystemName(_region));
            
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = name,
                InputStream = compressedImage,
                ContentType = contentType
            };
            
            var response = await client.PutObjectAsync(putRequest);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var url = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{name}";
                _logger.LogInformation("Image uploaded successfully: {FileName}", filename);
                return url;
            }
            
            _logger.LogError("Failed to upload image {Filename} to S3. Status code: {StatusCode}", filename, response.HttpStatusCode);
            throw new Exception($"Failed to upload image to S3: {response.HttpStatusCode}");
        }
        private static string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                _ => "image/jpeg"
            };
        }

        private async Task<Stream> CompressImageAsync(Stream imageStream, string filename)
        {
            try
            {
                var outputStream = new MemoryStream();
                
                // Use 'using' to properly dispose the image
                using var image = await Image.LoadAsync(imageStream);
                
                var originalWidth = image.Width;
                var originalHeight = image.Height;
                var (newWidth, newHeight) = CalculateDimensions(originalWidth, originalHeight);
                
                if (newWidth != originalWidth || newHeight != originalHeight)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(newWidth, newHeight),
                        Mode = ResizeMode.Max
                    }));
                    
                    _logger.LogInformation(
                        "Image resized from {OriginalWidth}x{OriginalHeight} to {NewWidth}x{NewHeight}",
                        originalWidth, originalHeight, newWidth, newHeight);
                }
                
                var extension = Path.GetExtension(filename).ToLowerInvariant();
                
                if (extension == ".png")
                {
                    await image.SaveAsync(outputStream, new PngEncoder());
                }
                else
                {
                    await image.SaveAsync(outputStream, new JpegEncoder { Quality = Quality });
                }
                
                outputStream.Position = 0;
                _logger.LogInformation("Image compressed successfully: {FileName}", filename);
                return outputStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compressing image {Filename}", filename);
                imageStream.Position = 0; // Reset position before returning
                return imageStream;
            }
        }
        private (int width, int height) CalculateDimensions(int originalWidth, int originalHeight)
        {
            // If image is already small enough, don't resize
            if (originalWidth <= MaxWidth && originalHeight <= MaxHeight)
            {
                return (originalWidth, originalHeight); // Return original size
            }

            // Calculate scaling ratios
            var widthRatio = (double)MaxWidth / originalWidth;   // e.g., 1920 / 4000 = 0.48
            var heightRatio = (double)MaxHeight / originalHeight; // e.g., 1920 / 3000 = 0.64

            // Use the SMALLER ratio to ensure both dimensions fit
            var ratio = Math.Min(widthRatio, heightRatio); // 0.48 (the smaller one)

            // Apply ratio to get new dimensions
            var newWidth = (int)(originalWidth * ratio);   // 4000 * 0.48 = 1920
            var newHeight = (int)(originalHeight * ratio); // 3000 * 0.48 = 1440

            return (newWidth, newHeight);
        }
    }
}
