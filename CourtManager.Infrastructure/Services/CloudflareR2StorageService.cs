using Amazon.S3;
using Amazon.S3.Model;
using CourtManager.Application.Interfaces;

using Microsoft.Extensions.Configuration;

namespace CourtManager.Infrastructure.Services;

public class CloudflareR2StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly string _bucketName;
    private readonly string _publicDomain;

    public CloudflareR2StorageService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _configuration = configuration;
        
        _bucketName = _configuration["CloudflareR2:BucketName"] 
            ?? throw new ArgumentNullException("CloudflareR2:BucketName");
        _publicDomain = _configuration["CloudflareR2:PublicDomain"] 
            ?? throw new ArgumentNullException("CloudflareR2:PublicDomain");
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderName, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream is empty", nameof(fileStream));
        }

        var extension = Path.GetExtension(fileName);
        var objectKey = $"{folderName}/{Guid.NewGuid()}{extension}";
        
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        // Remove trailing slash if present in PublicDomain
        var domain = _publicDomain.TrimEnd('/');
        return $"{domain}/{objectKey}";
    }

    public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        var domain = _publicDomain.TrimEnd('/');
        if (fileUrl.StartsWith(domain))
        {
            var objectKey = fileUrl.Substring(domain.Length).TrimStart('/');
            
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
        }
    }
}
