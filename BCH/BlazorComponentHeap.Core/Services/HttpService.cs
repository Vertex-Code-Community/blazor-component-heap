using System.Text;
using System.Globalization;
using Newtonsoft.Json;
using BlazorComponentHeap.Core.Services.Interfaces;

namespace BlazorComponentHeap.Core.Services;

public class HttpService : IHttpService
{
    public async Task<TResult?> PostAsync<TResult, TRequest>(string uri, TRequest body)
        where TResult : class
        where TRequest : class
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(uri) };
        var request = new HttpRequestMessage(HttpMethod.Post, "");
        
        var dateTimeNow = DateTime.Now;
        
        request.Headers.Add("X-Id", GenerateCode(dateTimeNow));
        request.Headers.Add("Time", dateTimeNow.ToString(CultureInfo.InvariantCulture));
        
        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        try
        {
            using var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                //var errorMessage = await response.Content.ReadAsStringAsync();
                return null;
            }

            object content = await response.Content.ReadAsStringAsync();
            
            if (typeof(TResult) == typeof(string))
            {
                return (TResult) content;
            }

            return JsonConvert.DeserializeObject<TResult>((string) content)!;
        }
        catch
        { 
        }

        return null;
    }
    
    private string GenerateCode(DateTime date)
    {
        var guild = Guid.NewGuid().ToString()[..^2];
        var randomValue = date.Second;
        var res = guild + randomValue.ToString("X2").ToLower();

        return res;
    }
}
