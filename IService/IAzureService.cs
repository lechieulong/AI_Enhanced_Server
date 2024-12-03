using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IAzureService
    {
        Task<string> ExtractTextFromImageAsync(Stream imageStream);
        Task<string> ProcessAndTranscribeAudioFromBlobAsync(string fileUrl);
    }
}
