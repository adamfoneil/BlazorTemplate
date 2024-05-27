using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

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

	protected virtual async Task<bool> ThrowExceptionAsync(HttpResponseMessage? response, Exception exception, [CallerMemberName] string? methodName = null) => await Task.FromResult(true);

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
			if (!await ThrowExceptionAsync(response, exc)) return default;				
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
			if (!await ThrowExceptionAsync(response, exc)) return default;
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
			if (!await ThrowExceptionAsync(response, exc)) return;
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
			if (!await ThrowExceptionAsync(response, exc)) return default;
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
			if (!await ThrowExceptionAsync(response, exc)) return;
            throw;
		}
		finally
		{
			OnStopped(HttpMethod.Delete, uri);
		}
	}
}
