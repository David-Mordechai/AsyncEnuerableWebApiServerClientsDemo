using System.Text.Json;

using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5182");
try
{
    var cancellationTokenSource = new CancellationTokenSource();
    cancellationTokenSource.CancelAfter(4000);

    var response = await client.GetAsync("WeatherForecast",
        HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token);

    response.EnsureSuccessStatusCode();

    await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token);
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultBufferSize = 128
    };

    var weathers =
        JsonSerializer.DeserializeAsyncEnumerable<WeatherForecast>(responseStream, options, cancellationTokenSource.Token);

    await foreach (var weather in weathers.WithCancellation(cancellationTokenSource.Token))
    {
        Console.WriteLine(weather);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation Canceled Exception");
}


Console.ReadKey();

internal record WeatherForecast(DateTime Date, int TemperatureC, string Summary);