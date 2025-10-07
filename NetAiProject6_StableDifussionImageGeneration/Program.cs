using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

class Program
{
    static async Task Main()
    {
        int i = 0;
        while(i<10)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:7860/");

            var payload = new
            {
                // Oluşturulacak görüntü için pozitif prompt
                prompt = "A photorealistic portrait of a young woman with long hair, wearing a flowing dress, standing in a sunlit forest, ultra-detailed, 8k, realistic lighting, cinematic composition",

                // Modelin üretmesini istemediğiniz şeyler
                negative_prompt = "blurry, low quality, deformed face, bad anatomy, text, watermark, cartoonish, extra limbs",

                // Kaç adımda üretim yapılacak
                steps = 25,

                // Görüntü boyutları
                width = 512,
                height = 512,

                // Prompt uyum seviyesi
                cfg_scale = 7.5
            };



            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("sdapi/v1/txt2img", content);

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("API Yanıtı:");
            Console.WriteLine(result);

            // JSON yanıtını çöz
            dynamic jsonResult = JsonConvert.DeserializeObject(result);
            string b64Image = jsonResult.images[0];

            // Base64'ü byte dizisine çevir
            byte[] imageBytes = Convert.FromBase64String(b64Image);

            // Masaüstü yolunu al
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"cikti1524{i}.png");

            // Görseli kaydet
            File.WriteAllBytes(filePath, imageBytes);

            Console.WriteLine($"Görsel kaydedildi: {filePath}");
            i++;
        }
 
    }
}
