using Newtonsoft.Json;
using System.Text;
using UglyToad.PdfPig;

class Program
{
    private static readonly string apiKey = "XXXXXXXXXXXXXXX";

    static async Task Main(string[] args)
    {
        Console.WriteLine("PDF Dosya Yolunu Giriniz: ");
        string pdfPath = Console.ReadLine();
        string pdfText = ExtractTextFromPdf(pdfPath);
        await AnalyzeWithAI(pdfText, "PDF");

        static string ExtractTextFromPdf(string pdfPath)
        {
            StringBuilder text = new StringBuilder();
            using (PdfDocument pdf = PdfDocument.Open(pdfPath))
            {
                foreach (var page in pdf.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }
            return text.ToString();
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
                        new {
                            role = "system",
                            content = "You are an AI assistant who summarizes PDF documents. Your answers always should be in turkish"
                        },
                        new {
                            role = "user",
                            content = "Please summarize the following relevant text:\n\n" + text
                        }
                    }
                };


                string json = JsonConvert.SerializeObject(requestBody);

                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

                    Console.WriteLine($"\n AI Analizi({sourceType}): \n {result.choices[0].message.content}");
                }
                else
                {
                    Console.WriteLine("Hata: " + responseJson);
                }
            }
        }
    }
}