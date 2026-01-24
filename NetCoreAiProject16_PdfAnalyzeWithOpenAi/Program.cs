using Newtonsoft.Json; // JSON serialization library
using System.Text;
using UglyToad.PdfPig; // PDF parsing library

class Program
{
    private static readonly string apiKey = "XXXXXXXXXXXXXXX"; // OpenAI API key

    static async Task Main(string[] args)
    {
        Console.WriteLine("PDF Dosya Yolunu Giriniz: ");
        string pdfPath = Console.ReadLine(); // Get PDF file path from user
        string pdfText = ExtractTextFromPdf(pdfPath); // Extract text from PDF
        await AnalyzeWithAI(pdfText, "PDF"); // Analyze PDF content with AI

        // Extract text content from PDF file using PdfPig library
        static string ExtractTextFromPdf(string pdfPath)
        {
            StringBuilder text = new StringBuilder(); // Create string builder for extracted text
            using (PdfDocument pdf = PdfDocument.Open(pdfPath)) // Open PDF document
            {
                foreach (var page in pdf.GetPages()) // Iterate through all pages
                {
                    text.AppendLine(page.Text); // Append page text to StringBuilder
                }
            }
            return text.ToString(); // Return extracted text
        }

        // Analyze extracted text using OpenAI API
        static async Task AnalyzeWithAI(string text, string sourceType)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}"); // Add API key to headers

                // Create request body for OpenAI API
                var requestBody = new
                {
                    model = "gpt-3.5-turbo", // GPT-3.5 model for text analysis
                    messages = new[]
                    {
                        // System prompt: Defines AI assistant role (Turkish response required)
                        new {
                            role = "system",
                            content = "You are an AI assistant who summarizes PDF documents. Your answers always should be in turkish"
                        },
                        // User prompt: Request to summarize the PDF text
                        new {
                            role = "user",
                            content = "Please summarize the following relevant text:\n\n" + text
                        }
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

                    // Display AI analysis result
                    Console.WriteLine($"\n AI Analizi({sourceType}): \n {result.choices[0].message.content}");
                }
                else
                {
                    Console.WriteLine("Hata: " + responseJson); // Display error message in Turkish
                }
            }
        }
    }
}