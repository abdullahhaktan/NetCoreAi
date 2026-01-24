using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Speech.Recognition;

class Program
{
    static async Task Main()
    {
        var recognizer = new SpeechRecognitionEngine();

        recognizer.SetInputToDefaultAudioDevice();
        recognizer.LoadGrammar(new DictationGrammar());

        while (true)
        {
            Console.WriteLine("Sorunuzu sorunuz çıkış için exit deyin...");

            var result = recognizer.Recognize();
            if (result == null)
            {
                Console.WriteLine("Anlaşılamadı, lütfen tekrar deneyin.");
                continue;
            }

            string prompt = result.Text;
            Console.WriteLine($"Anlaşılan: {prompt}");

            if (prompt.ToLower() == "exit")
            {
                return;
            }
            string ollamaUrl = "http://localhost:11434/api/chat";
            string modelName = "llama2";

            using var client = new HttpClient();

            var requestBody = new
            {
                model = modelName,
                messages = new[]
                {
                    new { role = "user", content = prompt },
                    new { role = "system", content = "You are a helpful assistant." }
                }
            };

            string jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(ollamaUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Ollama Cevabı: ");
                    var lines = responseString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    var finalAnswer = new StringBuilder();

                    foreach (var line in lines)
                    {
                        try
                        {
                            var json = JsonSerializer.Deserialize<JsonElement>(line);
                            if (json.TryGetProperty("message", out var message) &&
                                message.TryGetProperty("content", out var content1))
                            {
                                finalAnswer.Append(content1.GetString());
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    Console.WriteLine(finalAnswer.ToString());
                }
                else
                {
                    Console.WriteLine($"Bir hata oluştu:  {response.StatusCode}");
                    Console.WriteLine(responseString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
            }
        }
    }
}