using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Schema;

class Program
{
    private static readonly string apiKey = "XXXXXXX";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Lütfen analiz yapmak istediğiniz web sayfasının URL'sini giriniz: ");
        string inputUrl = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Web sayfası içeriği: ");
        string webContent = ExtractTextFromWeb(inputUrl);
        await AnalyzeWithAI(webContent, "web sayfası");
    }

    static string ExtractTextFromWeb(string inputUrl)
    {
        var web = new HtmlWeb();
        var doc = web.Load(inputUrl);

        var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText;
        return bodyText ?? "Sayfa içeriği okunamadı.";
    }

    static string WrapTextToConsoleWidth(string text)
    {
        int consoleWidth = Console.WindowWidth;

        int maxLineLength = consoleWidth > 5 ? consoleWidth - 2 : 78;

        StringBuilder wrapped = new StringBuilder();
        for (int i = 0; i < text.Length; i += maxLineLength)
        {
            int length = Math.Min(maxLineLength, text.Length - i);
            string part = text.Substring(i, length);

            if (i + length < text.Length)
                wrapped.AppendLine(part + "-");
            else
                wrapped.AppendLine(part);
        }

        return wrapped.ToString();
    }

    static async Task AnalyzeWithAI(string text, string sourceType)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new {role = "system" , content = "Sen bir yapay zeka asistanısın. Kullanıcının  gönderdiği metni analiz eder ve Türkçe olarak özetlersin. Yanıtlarını sadece türkçe ver!"},
                   new {role = "user",content = $"Analyze and summarize the following {sourceType}: \n\n {text}"}
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            HttpContent content = new StringContent(json,Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

                string aiContent = result.choices[0].message.content.ToString();
                string wrappedText = WrapTextToConsoleWidth(aiContent);

                Console.WriteLine($"\n AI Analizi ({sourceType}): \n {wrappedText}");
            }
            else
            {
                Console.WriteLine($"Hata: {responseJson}");
            }
        }
    }
}