using Newtonsoft.Json;
using System.Text;

class Program
{
    private static readonly string apiKey = "XXXXXXXXXXXXXXXXXXXXXXX"; // OpenAI API key for text summarization

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Uzun metninizi veya makalenizi giriniz: ");
        string input;
        input = Console.ReadLine(); // Get long text input from user

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine();
            Console.WriteLine("Girmiş olduğunuz metin AI tarafından özetleniyor...");
            Console.WriteLine();

            // Generate three different summaries: short, medium, and detailed
            string shortSummary = await SummarizeText(input, "short");

            string mediumSummary = await SummarizeText(input, "medium");

            string detailedSummary = await SummarizeText(input, "detailed");

            // Display all three summaries
            Console.WriteLine("Özetler");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($" ** Kısa Özet: **{shortSummary}");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($" ** Orta Uzunlukta Özet : **{mediumSummary}");
            Console.WriteLine("-------------------");
            Console.WriteLine($" ** Detaylı Özet:{detailedSummary} **");
        }

        Console.ReadLine(); // Keep console window open
    }

    // Method to summarize text at different detail levels
    public static async Task<string> SummarizeText(string text, string level)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // Determine summary length based on level parameter
            string instruction = level switch
            {
                "short" => "Summarize this text in 1-2 sentences.",
                "medium" => "Summarize this text in 3-5 sentences.",
                "detailed" => "Summarize this text in detailed but concise manner.",
                _ => "Summarize this text."
            };

            var requestBody = new
            {
                model = "gpt-3.5-turbo", // OpenAI model for summarization
                messages = new[]
                {
                    // System message defines AI's role
                    new {role="system",content = "You are an AI that summarize text info different levels: short , medium and detailed."},
                    // User message contains instruction and text to summarize
                    new {role="user",content=$"{instruction} \n\n {text}"}
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send request to OpenAI API
            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Parse JSON response using dynamic type
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

                return result.choices[0].message.content; // Return summary text
            }
            else
            {
                Console.WriteLine($"Hata: {responseJson}"); // Error message in Turkish
                return "Hata!"; // Return "Error!" in Turkish
            }
        }
    }
}