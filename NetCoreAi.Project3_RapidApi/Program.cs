using NetCoreAi.Project3_RapidApi.ViewModels;
using Newtonsoft.Json;


var client = new HttpClient(); // Create HttpClient instance for API calls
List<ApiSeriesViewModel> apiSeriesViewModel = new List<ApiSeriesViewModel>(); // List to store series data

var request = new HttpRequestMessage
{
    Method = HttpMethod.Get,
    RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/series/"), // RapidAPI endpoint for top series
    Headers =
    {
        { "x-rapidapi-key", "XXXXXXXXXXXXXXXXXXXXX" }, // API key (should be stored securely)
        { "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" }, // API host
    },
};

// Send API request and process response
using (var response = await client.SendAsync(request))
{
    response.EnsureSuccessStatusCode(); // Throw exception if request failed
    var body = await response.Content.ReadAsStringAsync(); // Read response content as string

    // Deserialize JSON response to list of ApiSeriesViewModel objects
    apiSeriesViewModel = JsonConvert.DeserializeObject<List<ApiSeriesViewModel>>(body);

    // Display each series information in console
    foreach (var series in apiSeriesViewModel)
    {
        Console.WriteLine(series.rank + "-" + series.title + "- Film Puanı: " + series.rating + "-Yapım Yılı:" + series.year);
    }
}

Console.ReadLine(); // Keep console window open