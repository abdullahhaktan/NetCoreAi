
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        /*  OLLAMA CHATBOT İLE KULLANIM  */


        while (true)
        {
            Console.WriteLine("Lütfen Sorunuzu giriniz (Örneğin istanbulda hava kaç derece:?) (çıkış için exit yazın)");

            string prompt = Console.ReadLine();

            string ollamaUrl = "http://localhost:11434/api/chat";
            string modelName = "llama2";

            using var client = new HttpClient();

            var requestBody = new
            {
                model = modelName,
                messages = new[]
                {
                new { role="user",content = prompt},
                new { role="system",content = "You are a helpful assistant."}
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


        /* ******************************** OPEN Aİ APİSİ İLE KULLANIM   ************************************  */

        //var apiKey = "XXXXXXXXXXXXXXXXXXX";
        //Console.WriteLine("Lütfen sorunuzu yazınız: (Örnek merhaba bugün istanbulda hava kaç derece)");

        //var prompt = Console.ReadLine();
        //using var httpClient = new HttpClient();
        //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        //var requestBody = new
        //{
        //    model = "gpt-3.5-turbo",
        //    messages = new[]
        //    {
        //        new {role="system",content="You are a helpful assistant."},
        //        new {role="user",content=prompt}
        //    },
        //    max_tokens = 100
        //};

        //var json = JsonSerializer.Serialize(requestBody);

        //var content = new StringContent(json,Encoding.UTF8, "application/json");

        //try
        //{
        //    var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        //    var responseString = await response.Content.ReadAsStringAsync();
        //    if(response.IsSuccessStatusCode)
        //    {
        //        var result = JsonSerializer.Deserialize<JsonElement>(responseString);
        //        var answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        //        Console.WriteLine("Open Ai Cevabı: ");
        //        Console.WriteLine(answer);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Bir hata oluştu:  {response.StatusCode}");
        //        Console.WriteLine(responseString);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        //}



        //// Gemini API anahtarınızı buraya girin. Google AI Studio'dan alabilirsiniz.
        //var apiKey = "sk-83eb3e14056d49cfb8fb3555946dfa27";

        //Console.WriteLine("Lütfen sorunuzu yazınız: (Örnek: Bugün İstanbul'da hava nasıl?)");
        //var prompt = Console.ReadLine();

        //// Gemini API'sine istek göndermek için HttpClient sınıfı kullanılır.
        //using var httpClient = new HttpClient();
        //var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        //// Gemini API'sinin beklediği JSON formatı.
        //var requestBody = new
        //{
        //    contents = new[]
        //    {
        //        new { parts = new[] { new { text = prompt } } }
        //    },
        //    generationConfig = new
        //    {
        //        maxOutputTokens = 100 // Yanıtın maksimum uzunluğunu belirler.
        //    }
        //};

        //var json = JsonSerializer.Serialize(requestBody);
        //var content = new StringContent(json, Encoding.UTF8, "application/json");

        //try
        //{
        //    // API'ye POST isteği gönderilir.
        //    var response = await httpClient.PostAsync(requestUrl, content);
        //    var responseString = await response.Content.ReadAsStringAsync();

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Gelen JSON yanıtı ayrıştırılır.
        //        var result = JsonSerializer.Deserialize<JsonElement>(responseString);
        //        var answer = result
        //            .GetProperty("candidates")[0]
        //            .GetProperty("content")
        //            .GetProperty("parts")[0]
        //            .GetProperty("text")
        //            .GetString();

        //        Console.WriteLine("Gemini'nin Cevabı:");
        //        Console.WriteLine(answer);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Bir hata oluştu: {response.StatusCode}");
        //        Console.WriteLine(responseString);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        //}


    }
}