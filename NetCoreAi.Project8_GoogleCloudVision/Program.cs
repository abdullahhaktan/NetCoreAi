using Google.Cloud.Vision.V1; // Google Cloud Vision API client library

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Görüntü yolunu girin:");
        Console.WriteLine();
        string imagePath = Console.ReadLine(); // Get image path from user

        // Path to Google Cloud service account credentials JSON file
        string credentialPath = @"C:\Users\abdullahhaktan\OneDrive - Ataturk University\Masaüstü\mynetaiproject-28a76ee148cb.json";

        // Set environment variable for Google Cloud authentication
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

        try
        {
            // Create Google Cloud Vision API client (auto-authenticates with credentials)
            var client = ImageAnnotatorClient.Create();

            // Load image from file
            var image = Image.FromFile(imagePath);

            // Call Vision API to detect text in image (OCR operation)
            var response = client.DetectText(image);

            Console.WriteLine("Görseldeki metin:");

            // Iterate through text annotations returned by API
            foreach (var annotation in response)
            {
                if (!string.IsNullOrEmpty(annotation.Description))
                {
                    Console.WriteLine(annotation.Description); // Print detected text
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }
    }
}