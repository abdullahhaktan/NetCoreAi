using System.Speech.Synthesis; // Windows built-in speech synthesis namespace

class Program
{
    static void Main(string[] args)
    {
        // Create SpeechSynthesizer instance (Text-to-Speech engine)
        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        // Set speech properties
        speechSynthesizer.Volume = 100; // Volume range: 0 (silent) to 100 (maximum)
        speechSynthesizer.Rate = 3; // Speech rate: -10 (slowest) to 10 (fastest), 0 = normal

        Console.WriteLine("Giriş: ");
        string input;
        input = Console.ReadLine(); // Get text input from user

        if (!string.IsNullOrEmpty(input))
        {
            // Convert text to speech (synchronous operation)
            speechSynthesizer.Speak(input);
        }

        Console.ReadLine(); // Keep console open
    }
}