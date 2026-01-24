using NAudio.Wave;
using System.Text;
using System.Text.Json;
using Vosk;

class Program
{
    // Ollama API endpoint and API key if needed
    private static readonly string OllamaEndpoint = "http://localhost:11434/api/chat";

    static async Task Main(string[] args)
    {
        // Initialize Vosk speech recognition model (Turkish language model)
        Model model = new Model("C:\\vosk-model-small-tr-0.3");

        using var recognizer = new VoskRecognizer(model, 16000.0f);
        using var waveIn = new WaveInEvent();

        waveIn.WaveFormat = new WaveFormat(16000, 1); // 16 kHz mono format for speech recognition

        // Capture microphone data event handler
        waveIn.DataAvailable += async (sender, e) =>
        {
            // Process audio data when available
            if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var result = recognizer.Result(); // Get speech recognition result
                var prompt = ExtractText(result); // Extract text from JSON result
                if (!string.IsNullOrWhiteSpace(prompt))
                {
                    Console.WriteLine("Recognized Text: " + prompt);

                    try
                    {
                        string ollamaUrl = "http://localhost:11434/api/chat";
                        string modelName = "llama2";

                        using var client = new HttpClient();

                        // Create request body for Ollama API
                        var requestBody = new
                        {
                            model = modelName,
                            messages = new[]
                            {
                                new { role="user",content = prompt},
                                new { role="system",content = "You are a helpful Aİ assistant."}
                            }
                        };

                        if (prompt.ToLower() == "exit")
                        {
                            return;
                        }

                        string jsonBody = JsonSerializer.Serialize(requestBody);

                        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        try
                        {
                            var response = await client.PostAsync(ollamaUrl, content);

                            var responseString = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Ollama Yanıtı: ");

                                // Split response line by line (Ollama streams JSON lines)
                                var lines = responseString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                                var finalAnswer = new StringBuilder();

                                foreach (var line in lines)
                                {
                                    try
                                    {
                                        var json = JsonSerializer.Deserialize<JsonElement>(line);
                                        // 'out var' pattern: Extract values if properties exist
                                        if (json.TryGetProperty("message", out var message) &&
                                            message.TryGetProperty("content", out var content1))
                                        {
                                            finalAnswer.Append(content1.GetString());
                                        }
                                    }
                                    catch
                                    {
                                        // Ignore invalid JSON lines
                                        continue;
                                    }
                                }

                                // Print combined response
                                Console.WriteLine(finalAnswer.ToString());
                            }

                            else
                            {
                                Console.WriteLine($"Bir hata oluştu: {response.StatusCode}");
                                Console.WriteLine(responseString);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ollama hatası: " + ex.Message);
                    }
                }
            }
        };

        waveIn.StartRecording(); // Start capturing audio from microphone
        Console.WriteLine("Kaydediliyor... Durdurmak için Enter tuşuna basın.");
        Console.WriteLine("Lütfen sorunuzu girin (Örnek: İstanbul'da hava sıcaklığı kaç derece?) (çıkmak için 'exit' yazın)");

        // Program waits here, won't close until user presses Enter
        Console.ReadLine();

        waveIn.StopRecording(); // Stop audio recording
        Console.WriteLine("Kayıt durduruldu.");
    }

    // Extract only the text part from Vosk JSON result
    static string ExtractText(string voskResultJson)
    {
        using var doc = JsonDocument.Parse(voskResultJson);
        if (doc.RootElement.TryGetProperty("text", out var textProp))
        {
            return textProp.GetString();
        }
        return "";
    }
}