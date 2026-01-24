using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        /*  OLLAMA CHATBOT IMPLEMENTATION  */

        while (true)
        {
            Console.WriteLine("Lütfen sorunuzu girin (Örnek: İstanbul'da hava sıcaklığı kaç derece?) (çıkmak için 'exit' yazın)");

            string prompt = Console.ReadLine();

            string ollamaUrl = "http://localhost:11434/api/chat";
            string modelName = "llama2";

            using var client = new HttpClient(); // 'using var' - automatic Dispose, 'var' type inference

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
                    Console.WriteLine("Ollama Yanıtı: ");

                    // Split response line by line
                    var lines = responseString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    var finalAnswer = new StringBuilder();

                    foreach (var line in lines)
                    {
                        try
                        {
                            // 'var' keyword: Compiler automatically determines the type (JsonElement here)
                            var json = JsonSerializer.Deserialize<JsonElement>(line);

                            // 'out var' pattern: TryGetProperty method returns boolean, if successful assigns value to 'message' variable
                            // 'message' and 'content1' variables are declared in this line (via out parameter)
                            if (json.TryGetProperty("message", out var message) &&
                                message.TryGetProperty("content", out var content1))
                            {
                                // Variables declared with 'out var' are available in this scope
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


        /* ******************************** OPEN AI API IMPLEMENTATION   ************************************  */

        //var apiKey = "XXXXXXXXXXXXXXXXXXX";
        //Console.WriteLine("Please enter your question: (Example: Hello, what's the temperature in Istanbul today?)");

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

        //        Console.WriteLine("Open AI Response: ");
        //        Console.WriteLine(answer);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"An error occurred: {response.StatusCode}");
        //        Console.WriteLine(responseString);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"An error occurred: {ex.Message}");
        //}



        //// Gemini API key goes here. You can get it from Google AI Studio.
        //var apiKey = "sk-83eb3e14056d49cfb8fb3555946dfa27";

        //Console.WriteLine("Please enter your question: (Example: What's the weather like in Istanbul today?)");
        //var prompt = Console.ReadLine();

        //// HttpClient class is used to send requests to Gemini API.
        //using var httpClient = new HttpClient();
        //var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        //// JSON format expected by Gemini API.
        //var requestBody = new
        //{
        //    contents = new[]
        //    {
        //        new { parts = new[] { new { text = prompt } } }
        //    },
        //    generationConfig = new
        //    {
        //        maxOutputTokens = 100 // Determines maximum response length.
        //    }
        //};

        //var json = JsonSerializer.Serialize(requestBody);
        //var content = new StringContent(json, Encoding.UTF8, "application/json");

        //try
        //{
        //    // Send POST request to API.
        //    var response = await httpClient.PostAsync(requestUrl, content);
        //    var responseString = await response.Content.ReadAsStringAsync();

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Parse incoming JSON response.
        //        var result = JsonSerializer.Deserialize<JsonElement>(responseString);
        //        var answer = result
        //            .GetProperty("candidates")[0]
        //            .GetProperty("content")
        //            .GetProperty("parts")[0]
        //            .GetProperty("text")
        //            .GetString();

        //        Console.WriteLine("Gemini Response:");
        //        Console.WriteLine(answer);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"An error occurred: {response.StatusCode}");
        //        Console.WriteLine(responseString);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"An error occurred: {ex.Message}");
        //}


    }
}