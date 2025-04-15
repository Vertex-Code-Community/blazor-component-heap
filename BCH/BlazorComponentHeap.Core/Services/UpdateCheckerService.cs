using System.Text;
using Newtonsoft.Json;
using BlazorComponentHeap.Core.Models;
using BlazorComponentHeap.Core.Options;
using BlazorComponentHeap.Core.Providers;
using BlazorComponentHeap.Core.Services.Interfaces;

namespace BlazorComponentHeap.Core.Services;

internal class UpdateCheckerService : IUpdateCheckerService
{
    private readonly UpdateKeyHolder _updateKeyHolder;
    private readonly SecurityProvider _securityProvider;
    private readonly ILocalStorageService _storageService;
    private readonly IHttpService _httpService;

    public bool IsUpdateAvailable { get; private set; } = true;
    public Action? OnUpdate { get; set; }

    class DataPayload
    {
        public string Data { get; set; }
    }

    public UpdateCheckerService(
        UpdateKeyHolder updateKeyHolder,
        SecurityProvider securityProvider,
        ILocalStorageService storageService,
        IHttpService httpService)
    {
        _updateKeyHolder = updateKeyHolder;
        _securityProvider = securityProvider;
        _storageService = storageService;
        _httpService = httpService;

        Action? onCheckSubscription = async () =>
        {
            try
            {
                await CheckSubscriptionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        };
        onCheckSubscription.Invoke();
    }

    private async Task CheckSubscriptionAsync()
    {
        const string updateKey = "comp-lib-update-note";

        // 1. Check local storage time delay
        var lastCheckStr = await _storageService.GetItemAsync<string>(updateKey);
        var lastCheck = await _securityProvider.TryDecodeExpirationDateAsync(lastCheckStr);
        
        // Console.WriteLine($"lastCheck {lastCheck}");

        if (lastCheck is not null && (DateTime.Now - lastCheck.Value).TotalDays <= 7)
        {
            // Console.WriteLine("lastCheck not expired yet");
        
            return;
        }

        // 2. Send API request to check subscription state
        // Console.WriteLine("Send API request");
        
#if LOW_PERFORMACE
        return;
#endif

        // Console.WriteLine($"ASSEMBLY-NAME: {_updateKeyHolder.Assembly}");
        // Console.WriteLine($"UPDATE-KEY: {_updateKeyHolder.UpdateKey}");
        
        var response = await _httpService.PostAsync<string, DataPayload>(
            "https://blazor-component-heap.com/api/verification/chk-upd", 
            new DataPayload
            {
                Data = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(new UpdateKeyModel 
                { 
                    UpdateKey = _updateKeyHolder.UpdateKey,
                    AssemblyName = _updateKeyHolder.Assembly
                })))
            }
        );

        // 3. Cancel rendering if required
        if (response != null)
        {
            // Console.WriteLine($"API response parsing {response}");
            
            var decodedResult = _securityProvider.TryParseAPIResponse(response);

            if (decodedResult is not null) // custom parsing
            {
                // Console.WriteLine($"Token decoded: {decodedResult.Value}");

                IsUpdateAvailable = decodedResult.Value;

                if (decodedResult.Value)
                {
                    var encodedExpireTime = await _securityProvider.EncodeExpirationDateAsync(DateTime.Now);
                    await _storageService.SetItemAsync(updateKey, encodedExpireTime);
                }
                else
                {
                    await _storageService.SetItemAsync(updateKey, "");
                }
            }
            else
            {
                // Console.WriteLine("Token is invalid or corrupted");
                // Token is invalid or corrupted
                IsUpdateAvailable = false;
            }
        }
        else
        {
            // Console.WriteLine("Server not responding, no connection");
            // Server not responding, no connection
            IsUpdateAvailable = true;
        }

        OnUpdate?.Invoke();
    }
}
