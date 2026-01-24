using Tesseract; // OCR (Optical Character Recognition) library

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Karakter tanıma için görüntü yolunu girin:");
        string imagePath = Console.ReadLine(); // Get image path from user
        Console.WriteLine();

        string tessDataPath = @"C:\tessdata"; // Path to Tesseract language data files

        try
        {
            // Initialize Tesseract OCR engine with English language
            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            {
                // Load image from file path
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    // Process image with OCR engine
                    using (var page = engine.Process(img))
                    {
                        string text = page.GetText(); // Extract text from image
                        Console.WriteLine("Görüntüden çıkarılan metin...");
                        Console.WriteLine(text);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }
}