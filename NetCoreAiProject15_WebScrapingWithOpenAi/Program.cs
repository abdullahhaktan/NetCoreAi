using HtmlAgilityPack; // HTML parsing library
using Newtonsoft.Json; // JSON serialization library
using System.Text;
using System.Xml.Schema;

class Program
{
    private static readonly string apiKey = "XXXXXXX"; // OpenAI API key

    static async Task Main(string[] args)
    {
        Console.WriteLine("Lütfen analiz yapmak istediğiniz web sayfasının URL'sini giriniz: ");
        string inputUrl = Console.ReadLine(); // Get URL from user

        Console.WriteLine();
        Console.WriteLine("Web sayfası içeriği: ");
        string webContent = ExtractTextFromWeb(inputUrl); // Extract text from webpage
        await AnalyzeWithAI(webContent, "web sayfası"); // Analyze content with AI
    }

    // Extract text content from webpage using HTML Agility Pack
    static string ExtractTextFromWeb(string inputUrl)
    {
        var web = new HtmlWeb(); // Create HTML web loader
        var doc = web.Load(inputUrl); // Load HTML document from URL

        // Select body element and extract inner text
        var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText;
        return bodyText ?? "Sayfa içeriği okunamadı."; // Return text or error message
    }

    // Format text to fit console window width (word wrapping)
    static string WrapTextToConsoleWidth(string text)
    {
        int consoleWidth = Console.WindowWidth; // Get current console width

        int maxLineLength = consoleWidth > 5 ? consoleWidth - 2 : 78; // Calculate max line length

        StringBuilder wrapped = new StringBuilder(); // Create string builder for wrapped text
        for (int i = 0; i < text.Length; i += maxLineLength)
        {
            int length = Math.Min(maxLineLength, text.Length - i); // Calculate substring length
            string part = text.Substring(i, length); // Extract part of text

            if (i + length < text.Length)
                wrapped.AppendLine(part + "-"); // Add hyphen for line continuation
            else
                wrapped.AppendLine(part); // Add last part without hyphen
        }

        return wrapped.ToString(); // Return wrapped text
    }

    // Analyze text content using OpenAI API
    static async Task AnalyzeWithAI(string text, string sourceType)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}"); // Add API key to headers

            // Create request body for OpenAI API
            var requestBody = new
            {
                model = "gpt-3.5-turbo", // GPT-3.5 model for analysis
                messages = new[]
                {
                    // System prompt: AI assistant role definition (Turkish response requested)
                    new {role = "system" , content = "Sen bir yapay zeka asistanısın. Kullanıcının  gönderdiği metni analiz eder ve Türkçe olarak özetlersin. Yanıtlarını sadece türkçe ver!"},
                   // User prompt: Request to analyze and summarize content
                   new {role = "user",content = $"Analyze and summarize the following {sourceType}: \n\n {text}"}
                }
            };

            string json = JsonConvert.SerializeObject(requestBody); // Serialize to JSON
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json"); // Create HTTP content

            // Send POST request to OpenAI API
            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            string responseJson = await response.Content.ReadAsStringAsync(); // Read response as string

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson); // Parse JSON response

                string aiContent = result.choices[0].message.content.ToString(); // Get AI response text
                string wrappedText = WrapTextToConsoleWidth(aiContent); // Format for console display

                Console.WriteLine($"\n AI Analizi ({sourceType}): \n {wrappedText}"); // Display AI analysis
            }
            else
            {
                Console.WriteLine($"Hata: {responseJson}"); // Display error message
            }
        }
    }
}