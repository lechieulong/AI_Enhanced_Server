using System.IO;
using System.Threading.Tasks;

namespace IService
{
    public interface IBlogStorageService
    {
        Task<string> UploadFileAsync(string userId, Stream fileStream, string fileName, string contentType);
        Task<string> UploadFileCourseAsync(string containerName, string path, Stream fileStream, string fileName, string contentType, bool useCourseStorage = false);
        Task<string> DownloadTemplate();

    }
}
