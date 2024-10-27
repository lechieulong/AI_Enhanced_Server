using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IService;

public class BlobStorageService : IBlogStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(string userId, Stream fileStream, string fileName, string contentType)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(userId);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

        return blobClient.Uri.ToString();
    }

    public async Task<string> DownloadTemplate()
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("template");

        var blobClient = blobContainerClient.GetBlobClient("QuestionTemplate.xlsx");

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException("The template file does not exist.");
        }
        return blobClient.Uri.ToString();
    }

}
