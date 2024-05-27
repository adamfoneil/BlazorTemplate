﻿using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ApiClientBaseLibrary;

/// <summary>
/// since Refit doesn't seem to work with authentication in Blazor WebAssembly, this is kind of the next best thing
/// </summary>
public abstract class ApiClientBase(HttpClient httpClient, ILogger<ApiClientBase> logger)
{
    protected readonly ILogger<ApiClientBase> Logger = logger;

    protected HttpClient Client { get; } = httpClient;

    protected virtual void OnStarted(HttpMethod method, string uri) { }

    protected virtual void OnStopped(HttpMethod method, string uri) { }

    protected async Task<T?> GetAsync<T>(string uri)
    {
        OnStarted(HttpMethod.Get, uri);
        var response = await Client.GetAsync(uri);

        try
        {
            //var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Error in {Method}", nameof(GetAsync));
            throw;
        }
        finally
        {
            OnStopped(HttpMethod.Get, uri);
        }
    }

    protected async Task<TResult?> PostWithResultAsync<TResult>(string uri)
    {
        OnStarted(HttpMethod.Post, uri);
        var response = await Client.PostAsync(uri, null);

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>();
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Error in {Method}", nameof(PostWithResultAsync));
            throw;
        }
        finally
        {
            OnStopped(HttpMethod.Post, uri);
        }
    }

    protected async Task PostWithInputAsync<T>(string uri, T value)
    {
        OnStarted(HttpMethod.Post, uri);
        var response = await Client.PostAsJsonAsync(uri, value);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Error in {Method}", nameof(PostWithInputAsync));
            throw;
        }
        finally
        {
            OnStopped(HttpMethod.Post, uri);
        }
    }

    protected async Task<TResult?> PostWithInputAndResultAsync<TResult>(string uri, TResult input)
    {
        OnStarted(HttpMethod.Post, uri);
        var response = await Client.PostAsJsonAsync(uri, input);

        try
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResult>();
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Error in {Method}", nameof(PostWithInputAndResultAsync));
            throw;
        }
        finally
        {
            OnStopped(HttpMethod.Post, uri);
        }
    }

    protected async Task DeleteAsync(string uri)
    {
        OnStarted(HttpMethod.Delete, uri);
        var response = await Client.DeleteAsync(uri);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception exc)
        {
            Logger.LogError(exc, "Error in {Method}", nameof(DeleteAsync));
            throw;
        }
        finally
        {
            OnStopped(HttpMethod.Delete, uri);
        }
    }
}
