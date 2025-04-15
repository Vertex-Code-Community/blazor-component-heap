namespace BlazorComponentHeap.Core.Options;

public class UpdateKeyHolder
{
    public UpdateKeyHolder(string key, string assembly)
    {
        UpdateKey = key;
        Assembly = assembly;
    }
    
    internal string UpdateKey { get; }
    internal string Assembly { get; }
}
