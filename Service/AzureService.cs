using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Text;
using IService;
using Azure.Storage.Blobs;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using System.Diagnostics;
using NAudio.Wave;

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

        public async Task<string> ExtractTextFromImageAsync(string imageUrl)
            {
                try
                {
                    // Create a HttpClient to download the image as a stream
                    using (var httpClient = new HttpClient())
                    {
                        // Download the image as a stream from the provided URI (imageUrl)
                        using (var imageStream = await httpClient.GetStreamAsync(imageUrl))
                        {
                            // Call the OCR API to process the image and recognize text
                            var ocrResult = await _visionClient.RecognizePrintedTextInStreamAsync(true, imageStream);

                            var resultText = new StringBuilder();

                            // Iterate through the recognized regions and extract text from each word
                            foreach (var region in ocrResult.Regions)
                            {
                                foreach (var line in region.Lines)
                                {
                                    foreach (var word in line.Words)
                                    {
                                        resultText.Append(word.Text + " "); // Concatenate the words
                                    }
                                    resultText.AppendLine(); // Add a new line after each line of text
                                }
                            }

                            return resultText.ToString(); // Return the recognized text
                        }
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}"; // Return error message if there's an exception
                }
            }




    public async Task<string> ProcessAndTranscribeAudioFromBlobAsync(string blobUri)
        {
            string localFilePath = await DownloadFileFromBlobAsync(blobUri);

            string wavFilePath = localFilePath;
            if (Path.GetExtension(localFilePath).ToLower() != ".wav")
            {
                wavFilePath =  ConvertToWav(localFilePath);
            }

            return await TranscribeAudioFromBlobAsync(wavFilePath);
        }

        private async Task<string> DownloadFileFromBlobAsync(string blobUri)
        {
            // Tạo BlobClient từ URL
            var blobClient = new BlobClient(new Uri(blobUri));

            // Kiểm tra blob có tồn tại không
            if (!await blobClient.ExistsAsync())
            {
                throw new Exception($"Blob không tồn tại: {blobUri}");
            }

            // Xác định tên file và tạo đường dẫn cục bộ
            string fileName = Path.GetFileName(blobClient.Uri.LocalPath);
            string localFilePath = Path.Combine(Path.GetTempPath(), fileName);

            // Tải xuống tệp
            await blobClient.DownloadToAsync(localFilePath);

            return localFilePath;
        }

        private string ConvertToWav(string inputFilePath)
        {
            string outputFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");

            try
            {
                // Ensure the file exists before processing
                if (!File.Exists(inputFilePath))
                {
                    throw new FileNotFoundException("Input audio file not found.", inputFilePath);
                }

                using (var reader = new AudioFileReader(inputFilePath))
                {
                    // Resample to mono, 16 kHz, 16-bit PCM
                    using (var resampler = new MediaFoundationResampler(reader, new WaveFormat(16000, 1)))
                    {
                        resampler.ResamplerQuality = 60; // Chất lượng chuyển đổi
                        WaveFileWriter.CreateWaveFile(outputFilePath, resampler);
                    }
                }

                Console.WriteLine($"Conversion successful: {outputFilePath}");
                return outputFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during conversion: {ex.Message}");
                throw new Exception("Error converting to WAV.", ex);
            }
        }

        private async Task<string> TranscribeAudioFromBlobAsync(string wavFilePath)
        {
            try
            {
                if (!File.Exists(wavFilePath))
                {
                    string error = $"WAV file not found: {wavFilePath}";
                    Console.WriteLine(error);
                    throw new FileNotFoundException(error, wavFilePath);
                }


                var speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
                speechConfig.SpeechRecognitionLanguage = "en-US"; // Customize language as needed

                using var audioConfig = AudioConfig.FromWavFileInput(wavFilePath);

                using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

                var resultText = new StringBuilder();

                recognizer.Recognizing += (s, e) =>
                {
                    Console.WriteLine($"Recognizing: {e.Result.Text}");
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        resultText.Append(e.Result.Text + " ");
                        Console.WriteLine($"Recognized: {e.Result.Text}");
                    }
                };

                // Event handler for cancellation (errors)
                recognizer.Canceled += (s, e) =>
                {
                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"Recognition canceled: {e.ErrorDetails}");
                    }
                };

                // Event handler when the session is stopped (i.e., recognition is complete)
                TaskCompletionSource<bool> sessionStoppedTcs = new TaskCompletionSource<bool>();
                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("Session stopped.");
                    sessionStoppedTcs.SetResult(true); // Set the TaskCompletionSource when the session is stopped
                };

                // Start continuous recognition asynchronously
                await recognizer.StartContinuousRecognitionAsync();

                // Wait for the session to stop (no fixed delay, just wait until recognition is done)
                await sessionStoppedTcs.Task;

                // Stop the recognition session after it has finished
                await recognizer.StopContinuousRecognitionAsync();

                // Return the transcribed text
                return resultText.ToString();
            }
            catch (Exception ex)
            {
                // Log detailed error message
                string detailedError = $"Error during transcription: {ex.Message}";
                Console.WriteLine(detailedError);
                throw new Exception("An error occurred during audio transcription.", ex);
            }
        }



    }
}
