using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DoeTech.DTOs;
using DoeTech.Services;

public class FileService : IService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "files";
    
    private readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(10);
    
    public FileService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    private string GetBlobName(FileType type, Guid id)
    {
        return $"{type.ToString().ToLower()}/{id}";
    }

    public async Task<string> UploadAsync(FileType type, Guid correlationId, IFormFile file)
    {
  
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedMimeTypes.Contains(file.ContentType))
            throw new InvalidOperationException("File type not supported");

        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

        var blobClient = containerClient.GetBlobClient(GetBlobName(type, correlationId));

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public string GetPresignedUrl(FileType type, Guid correlationId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(GetBlobName(type, correlationId));

        if (!blobClient.Exists())
            throw new InvalidOperationException($"File not found by type and correlationId: {type}, {correlationId}");

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(_expirationTime)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
