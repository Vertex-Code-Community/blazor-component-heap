namespace BlazorComponentHeap.Core.Providers;

internal class SecurityProvider
{
    private readonly EncryptProvider _encryptProvider;

    public SecurityProvider(EncryptProvider encryptProvider)
    {
        _encryptProvider = encryptProvider;
    }

    internal async Task<string> EncodeExpirationDateAsync(DateTime expiration)
    {
        return await _encryptProvider.EncryptAsync("secure-date-str", expiration);
    }

    internal async Task<DateTime> DecodeExpirationDateAsync(string expirationEncodedStr)
    {
        var dateTimeStr = await _encryptProvider.DecryptTextAsync("secure-date-str", expirationEncodedStr);
        var expiration = DateTime.Parse(dateTimeStr);

        return expiration;
    }

    internal async Task<DateTime?> TryDecodeExpirationDateAsync(string expirationEncodedStr)
    {
        if (string.IsNullOrEmpty(expirationEncodedStr))
        {
            return null!;
        }

        var dateTimeStr = await _encryptProvider.DecryptTextAsync("secure-date-str", expirationEncodedStr);
        
        return DateTime.TryParse(dateTimeStr, out var dateTime) ? dateTime : null;
    }

    internal bool? TryParseAPIResponse(string response)
    {
        if (response.Length < 2) return null;
        
        var twoLastSymbols = response.Substring(response.Length - 2, 2);
        var intValue = Convert.ToInt32(twoLastSymbols, 16);

        return intValue >= 8;
    }
}
