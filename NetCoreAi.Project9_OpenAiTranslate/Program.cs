using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Lütfen çevirmek istediğiniz cümleyi giriniz:");
        string inputText = Console.ReadLine();

        string apiKey = "XXXXXXXXXXXXXXXXXX";

        string translatedText = await TranslateTextToEnglish(inputText, apiKey);

        if (!string.IsNullOrEmpty(translatedText))
        {
            Console.WriteLine($"Çeviri (İngilizce): {translatedText}");
        }
        else
        {
            Console.WriteLine("Beklenmeyen bi hata oluştu");
        }
    }

    private static async Task<string> TranslateTextToEnglish(string text, string apiKey)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var requestBody = new
            {
                model = "gpt-4o-mini", // doğru model adı
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful translator." },
                    new { role = "user", content = $"Translate this text to English: {text}" }
                }
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject(responseString);

                if (responseObject?.choices != null && responseObject.choices.Count > 0)
                {
                    string translation = responseObject.choices[0].message.content.ToString();
                    return translation;
                }

                Console.WriteLine("API response beklenen formatta değil:");
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