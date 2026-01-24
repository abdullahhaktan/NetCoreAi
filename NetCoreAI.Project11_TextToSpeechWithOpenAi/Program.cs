using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

static class Program
{
    public static string apiKey = "XXXXXXXXXXXXXX"; // OpenAI API key for TTS (Text-to-Speech)

    static async Task Main(string[] args)
    {
        Console.Write("Enter Text: ");
        string input = Console.ReadLine(); // Get text input from user

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Creating audio file...");
            await GenerateSpeech(input); // Call OpenAI TTS API
            Console.WriteLine("Audio file saved as 'output.mp3'!");
            System.Diagnostics.Process.Start("explorer.exe", "output.mp3"); // Open file explorer to show file
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }

        // Method to generate speech using OpenAI TTS API
        static async Task GenerateSpeech(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                // Add API key to authorization header
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Request body for OpenAI TTS API
                var requestBody = new
                {
                    model = "tts-1", // OpenAI TTS model (tts-1 = standard, tts-1-hd = higher quality)
                    input = text, // Text to convert to speech
                    voice = "shimmer", // Voice option (others: alloy, echo, fable, onyx, nova)
                };

                string jsonBody = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send POST request to OpenAI TTS endpoint
                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

                if (response.IsSuccessStatusCode)
                {
                    // Read audio data as byte array (MP3 format)
                    byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync("output.mp3", audioBytes); // Save as MP3 file
                }
                else
                {
                    Console.WriteLine("An error occurred");
                }
            }
        }
    }
}