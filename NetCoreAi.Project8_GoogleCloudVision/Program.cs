using Google.Cloud.Vision.V1;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Resim yolunu giriniz:");
        Console.WriteLine();
        string imagePath = Console.ReadLine();

        string credentialPath = @"C:\Users\abdullahhaktan\OneDrive - Ataturk University\Masaüstü\mynetaiproject-28a76ee148cb.json";
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",credentialPath);

        try
        {
            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(imagePath);
            var response = client.DetectText(image);
            Console.WriteLine("Resimdeki Metin:");
            foreach(var annotation in response)
            {
                if(!string.IsNullOrEmpty(annotation.Description))
                {
                    Console.WriteLine(annotation.Description);
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu{ex.Message}");
        }
    }
}

