namespace BlazorComponentHeap.Core.Services.Interfaces;

public interface IHttpService
{
    Task<TResult?> PostAsync<TResult, TRequest>(string uriPrefix, TRequest body)
        where TResult : class
        where TRequest : class;
}
