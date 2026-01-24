using Newtonsoft.Json;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

class Program
{
    private static readonly string apiKey = "XXXXXXXXXXXXXXXXXX"; // OpenAI API key for sentiment analysis

    public static async Task Main(string[] args)
    {
        Console.Write("Please enter the text: ");
        string input;
        input = Console.ReadLine(); // Get text input from user for sentiment analysis

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine();
            Console.WriteLine("Performing sentiment analysis...");
            string sentiment = await AnalyzeSentiment(input); // Call sentiment analysis method

            Console.WriteLine($"Result: {sentiment}");
        }
    }

    static async Task<string> AnalyzeSentiment(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            // Add API key to authorization header
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            // Create request body for OpenAI Chat API
            var requestBody = new
            {
                model = "gpt-3.5-turbo", // GPT-3.5 model for sentiment analysis
                messages = new[]
                {
                    // System prompt defines the AI's role
                    new { role="system",content="You are an AI that analyzes sentiment. You categoryz text as Positive , negative or neutral . "},
                    // User prompt with text to analyze (note: "categoryz" typo in original)
                    new {role = "user",content=$"Analyze the sentiment of this text: \" {text} \" and return only Positive , negative or neutral"}
                }
            };

            string json = JsonConvert.SerializeObject(requestBody); // Convert to JSON

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send POST request to OpenAI API
            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            string responseJson = await response.Content.ReadAsStringAsync(); // Read response as string

            if (response.IsSuccessStatusCode)
            {
                // Deserialize JSON response using dynamic type
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                return result.choices[0].message.content; // Return sentiment analysis result
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}"); // Error message in Turkish
                return "Hata"; // Return "Error" in Turkish
            }
        }
    }
}