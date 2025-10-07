using Newtonsoft.Json;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

class Program
{
    private static readonly string apiKey = "XXXXXXXXXXXXXXXXXX";

    public static async Task Main(string[] args)
    {
        Console.Write("Lütfen metni giriniz: ");
        string input;
        input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine();
            Console.WriteLine("Duygu analizi yapılıyor...");
            string sentiment = await AnalyzeSentiment(input);

            Console.WriteLine($"Sonuç: { sentiment}");
        }
    }

    static async Task<string> AnalyzeSentiment(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role="system",content="You are an AI that analyzes sentiment. You categoryz text as Positive , negative or neutral . "},
                    new {role = "user",content=$"Analyze the sentiment of this text: \" {text} \" and return only Positive , negative or neutral"}
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions",content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                return result.choices[0].message.content;
            }
            else
            {
                Console.WriteLine($"Hata: {response.StatusCode}");
                return "Hata";
            }

        }
    }
}