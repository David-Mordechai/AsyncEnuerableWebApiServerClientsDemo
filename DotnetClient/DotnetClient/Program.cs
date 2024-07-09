using System.Text.Json;

using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5182");

var response = await client.GetAsync("WeatherForecast", HttpCompletionOption.ResponseHeadersRead);

response.EnsureSuccessStatusCode();

await using var responseStream = await response.Content.ReadAsStreamAsync();
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    DefaultBufferSize = 128
};

var weathers = JsonSerializer.DeserializeAsyncEnumerable<WeatherForecast>(responseStream, options);

await foreach (var weather in weathers)
{
    Console.WriteLine(weather);
}

Console.ReadKey();

internal record WeatherForecast(DateTime Date, int TemperatureC, string Summary);