using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

static class Program
{
    public static string apiKey = "XXXXXXXXXXXXXX";

    static async Task Main(string[] args)
    {
        Console.Write("Metni Giriniz: ");
        string input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Ses dosyası oluşturuluyor...");
            await GenerateSpeech(input);
            Console.WriteLine("Ses dosyası 'Output mp3' olarak kaydedildi!");
            System.Diagnostics.Process.Start("explorer.exe", "output.mp3");
        }
        else
        {
            Console.WriteLine("Geçersiz giriş.");
        }


        static async Task GenerateSpeech(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "tts-1",
                    input = text,
                    voice = "shimmer", // farklı modeller denenebilir voice için
                };

                string jsonBody = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

                if(response.IsSuccessStatusCode)
                {
                    byte[] audioBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync("output.mp3", audioBytes);
                }
                else
                {
                    Console.WriteLine("Bir hata oluştu");
                }
            }
        }
    }
}