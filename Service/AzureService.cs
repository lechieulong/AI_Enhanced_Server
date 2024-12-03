using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Text;
using IService;
using Azure.Storage.Blobs;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using System.Diagnostics;

namespace Service
{
    public class AzureService : IAzureService
    {
        private readonly string apiKey = "DNF18zNqjBGtitKePhRWMhQBaEtZ2BiGftBuQI7ldYYqUIdyoCKKJQQJ99ALACqBBLyXJ3w3AAAFACOGAalp";
        private readonly string endpoint = "https://computervisionresourcehydra.cognitiveservices.azure.com/";
        private readonly ComputerVisionClient _visionClient;
        private string subscriptionKey = "F8SeRNFIaj8X04HEyQv6HfeqcMEL0hfYisdGMd00NACJckxX0jetJQQJ99ALACqBBLyXJ3w3AAAYACOGD3km";
        private string region = "southeastasia";

        public AzureService()
        {
            _visionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey))
            {
                Endpoint = endpoint
            };
        }

        public async Task<string> ExtractTextFromImageAsync(Stream imageStream)
        {
            try
            {
                // Gọi API OCR để xử lý ảnh và nhận dạng văn bản
                var ocrResult = await _visionClient.RecognizePrintedTextInStreamAsync(true, imageStream);

                var resultText = new StringBuilder();

                // Duyệt qua các vùng chứa văn bản và trích xuất văn bản từ từng dòng
                foreach (var region in ocrResult.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            resultText.Append(word.Text + " "); // Gộp các từ lại với nhau
                        }
                        resultText.AppendLine(); // Thêm dòng mới sau mỗi dòng văn bản
                    }
                }

                return resultText.ToString(); // Trả về văn bản đã nhận dạng được
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}"; // Trả về thông báo lỗi nếu có
            }
        }



        public async Task<string> TranscribeAudioFromBlobAsync(string localFilePath)
        {
            // Check if file exists
            string wavFilePath = localFilePath;
            if (Path.GetExtension(localFilePath).ToLower() != ".wav")
            {
                wavFilePath = await ConvertToWavAsync(localFilePath);
            }

            // Ensure the file is valid WAV
            var speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
            var audioConfig = AudioConfig.FromWavFileInput(wavFilePath);

            var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                return result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                throw new Exception("No speech could be recognized.");
            }
            else
            {
                throw new Exception($"Speech recognition failed.");
            }
        }
   

        private async Task<string> ConvertToWavAsync(string inputFilePath)
        {
            string outputFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");

            // FFmpeg command to convert to WAV
            var ffmpegArgs = $"-i \"{inputFilePath}\" -ar 16000 -ac 1 -c:a pcm_s16le \"{outputFilePath}\"";

            using (var process = new Process())
            {
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = ffmpegArgs;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"FFmpeg conversion failed: {process.StandardError.ReadToEnd()}");
                }
            }

            return outputFilePath;
        }

    }
}
