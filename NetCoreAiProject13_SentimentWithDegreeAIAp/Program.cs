using Newtonsoft.Json;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json.Serialization;

class Program
{
    private static readonly string apiKey = "XXXXXXXXXXXXXXXXXXXX";
    
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Bir metin giriniz: ");
        string inputText;
        inputText = Console.ReadLine();
        Console.WriteLine();

        if(!string.IsNullOrEmpty(inputText))
        {
            Console.WriteLine("Gelişmiş duygu analizi yapılıyor...");
            string sentiment = await AdvancedSentimentalAnalysis(inputText);
            Console.WriteLine();
            Console.WriteLine($"\n Sonuç :\n{sentiment}");
        }
    }

    static async Task<string> AdvancedSentimentalAnalysis(string text)
    {
        using(HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new {role = "user" , content=$"You are an advanced AI that alayzes emotions in text. your response must be in JSON format. Identify the sentiment the scores (0-100%) for the following emotions: Joy , Sadness , Angry , Fear , Suprised and Neutral."},
                    new  {role="user",content=$"Analyze this text: \" {text} and return a JSON object with percentages for each emotions." }
                }
            };

            string Json = JsonConvert.SerializeObject(requestBody);

            HttpContent content = new StringContent(Json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                string analyzsis = result.choices[0].message.content;
                return analyzsis;
            }
            else
            {
                Console.WriteLine($"Bir Hata Oluştu: {responseJson}");
                return "Hata";
            }
        }
    }
}