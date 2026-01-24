using Newtonsoft.Json;
using System.Text;

class Program
{
    static async Task Main()
    {
        int i = 0;
        while (i < 10) // Generate 10 images
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:7860/"); // Stable Diffusion WebUI API endpoint

            // Stable Diffusion API request payload
            var payload = new
            {
                // Positive prompt for image generation
                prompt = "A photorealistic portrait of a young woman with long hair, wearing a flowing dress, standing in a sunlit forest, ultra-detailed, 8k, realistic lighting, cinematic composition",

                // Negative prompt - things to avoid in generation
                negative_prompt = "blurry, low quality, deformed face, bad anatomy, text, watermark, cartoonish, extra limbs",

                // Number of diffusion steps (higher = better quality but slower)
                steps = 25,

                // Image dimensions (512x512 is standard for many models)
                width = 512,
                height = 512,

                // CFG Scale - how closely to follow the prompt (higher = more strict)
                cfg_scale = 7.5
            };

            // Serialize payload to JSON and create HTTP content
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            // Send POST request to Stable Diffusion API txt2img endpoint
            var response = await client.PostAsync("sdapi/v1/txt2img", content);

            // Read API response
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("API Yanıtı:");
            Console.WriteLine(result);

            // Parse JSON response
            dynamic jsonResult = JsonConvert.DeserializeObject(result);
            string b64Image = jsonResult.images[0]; // Get first image from images array (Base64 encoded)

            // Convert Base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(b64Image);

            // Get desktop path for saving
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"cikti1524{i}.png"); // Generate unique filename

            // Save image to file
            File.WriteAllBytes(filePath, imageBytes);

            Console.WriteLine($"Görüntü kaydedildi: {filePath}");
            i++; // Increment counter for next image
        }
    }
}