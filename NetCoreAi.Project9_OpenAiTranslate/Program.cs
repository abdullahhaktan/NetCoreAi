using Newtonsoft.Json;
using System.Text;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Çevirmek istediğiniz cümleyi girin:");
        string inputText = Console.ReadLine(); // Get user input text to translate

        string apiKey = "XXXXXXXXXXXXXXXXXX"; // OpenAI API key (should be stored securely)

        string translatedText = await TranslateTextToEnglish(inputText, apiKey); // Call translation method

        if (!string.IsNullOrEmpty(translatedText))
        {
            Console.WriteLine($"Çeviri (İngilizce): {translatedText}");
        }
        else
        {
            Console.WriteLine("Beklenmedik bir hata oluştu");
        }
    }

    private static async Task<string> TranslateTextToEnglish(string text, string apiKey)
    {
        using (HttpClient client = new HttpClient())
        {
            // Add API key to request headers (Bearer token authentication)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // Create request body for OpenAI Chat Completions API
            var requestBody = new
            {
                model = "gpt-4o-mini", // Correct model name (smaller, cheaper version of GPT-4)
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful translator." }, // System prompt to set context
                    new { role = "user", content = $"Translate this text to English: {text}" } // User translation request
                }
            };

            // Serialize request to JSON
            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                // Send POST request to OpenAI API endpoint
                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                // Deserialize JSON response using dynamic type (flexible, no need for concrete class)
                dynamic responseObject = JsonConvert.DeserializeObject(responseString);

                // Check if response contains expected structure
                if (responseObject?.choices != null && responseObject.choices.Count > 0)
                {
                    string translation = responseObject.choices[0].message.content.ToString();
                    return translation;
                }

                Console.WriteLine("API yanıtı beklenen formatta değil:");
                Console.WriteLine(responseString);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                return null;
            }
        }
    }
}