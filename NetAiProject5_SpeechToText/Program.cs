using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vosk;
using NAudio.Wave;

class Program
{
    // Ollama API endpoint ve gerekiyorsa API key
    private static readonly string OllamaEndpoint = "http://localhost:11434/api/chat";

    static async Task Main(string[] args)
    {
        // Vosk modelini başlat
        Model model = new Model("C:\\vosk-model-small-tr-0.3");

        using var recognizer = new VoskRecognizer(model, 16000.0f);
        using var waveIn = new WaveInEvent();

        waveIn.WaveFormat = new WaveFormat(16000, 1); // 16 kHz mono

        // Mikrofon verisini yakalama
        waveIn.DataAvailable += async (sender, e) =>
        {
            if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var result = recognizer.Result();
                var prompt = ExtractText(result);
                if (!string.IsNullOrWhiteSpace(prompt))
                {
                    Console.WriteLine("Recognized Text: " + prompt);

                    try
                    {
                        string ollamaUrl = "http://localhost:11434/api/chat";
                        string modelName = "llama2";

                        using var client = new HttpClient();

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
                                Console.WriteLine("Ollama Cevabı: ");

                                // Yanıt satır satır ayrılıyor
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
                                        // Geçersiz JSON satırları yoksay
                                        continue;
                                    }
                                }

                                // Birleştirilmiş yanıtı yazdır
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
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ollama hatası: " + ex.Message);
                    }
                }
            }
        };

        waveIn.StartRecording();
        Console.WriteLine("Recording... Press Enter to stop.");
        Console.WriteLine("Lütfen Sorunuzu giriniz (Örneğin istanbulda hava kaç derece:?) (çıkış için exit yazın)");

        // Program burada bekler, kullanıcı Enter'a basana kadar kapanmaz
        Console.ReadLine();

        waveIn.StopRecording();
        Console.WriteLine("Recording stopped.");
    }

    // Vosk JSON sonucundan sadece text kısmını al
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
