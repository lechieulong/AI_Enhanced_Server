using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IService;

public class BlobStorageService : IBlogStorageService
{
    private readonly BlobServiceClient _blobServiceClient; // Service mặc định
    private readonly BlobServiceClient _courseServiceClient; // Service cho Course

    public BlobStorageService(string connectionString, string courseConnectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _courseServiceClient = new BlobServiceClient(courseConnectionString);
    }

    public async Task<string> UploadFileAsync(string userId, Stream fileStream, string fileName, string contentType)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(userId);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

        return blobClient.Uri.ToString();
    }

    public async Task<string> UploadFileCourseAsync(
        string containerName,
        string path,
        Stream fileStream,
        string fileName,
        string contentType,
        bool useCourseStorage = false)
    {
        var blobServiceClient = useCourseStorage ? _courseServiceClient : _blobServiceClient;
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        string blobPath = $"{path}/{fileName}";
        var blobClient = blobContainerClient.GetBlobClient(blobPath);

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
