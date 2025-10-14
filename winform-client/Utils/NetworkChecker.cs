public static class NetworkHelper
{
    public static bool IsOnline()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var result = client.GetAsync("https://www.google.com").Result;
            return result.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}
