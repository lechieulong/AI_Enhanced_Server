using System.IO;
using System.Threading.Tasks;

namespace IService
{
    public interface IBlogStorageService
    {
        Task<string> UploadFileAsync(string userId, Stream fileStream, string fileName, string contentType);
    }
}
