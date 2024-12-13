using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IAzureService
    {
        Task<string> ExtractTextFromImageAsync(string imageUrl);
        Task<string> ProcessAndTranscribeAudioFromBlobAsync(string fileUrl);
        Task<string> ProcessAndTranscribeAudioCreateInSkills(string fileUrl);
    }
}
