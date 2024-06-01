using Microsoft.Extensions.Logging;
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

	protected virtual void OnStopped(HttpMethod method, string uri, bool success) { }

	protected virtual void OnError(HttpMethod method, string uri, Exception exception) { }

	protected async Task<T?> GetAsync<T>(string uri)
	{
		OnStarted(HttpMethod.Get, uri);
		var response = await Client.GetAsync(uri);
		bool success = false;

		try
		{
			// uncomment this to debug unexpected responses
			//var content = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();			
			success = true;
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception exc)
		{
			Logger.LogError(exc, "Error in {Method}", nameof(GetAsync));
			OnError(HttpMethod.Get, uri, exc);			
		}
		finally
		{
			OnStopped(HttpMethod.Get, uri, success);
		}

		return default;
	}

	protected async Task<TResult?> PostWithResultAsync<TResult>(string uri)
	{
		OnStarted(HttpMethod.Post, uri);
		var response = await Client.PostAsync(uri, null);
		bool success = false;

		try
		{
			response.EnsureSuccessStatusCode();
			success = true;
			return await response.Content.ReadFromJsonAsync<TResult>();
		}
		catch (Exception exc)
		{
			Logger.LogError(exc, "Error in {Method}", nameof(PostWithResultAsync));
			OnError(HttpMethod.Post, uri, exc);
		}
		finally
		{
			OnStopped(HttpMethod.Post, uri, success);
		}

		return default;
	}

	protected async Task PostWithInputAsync<T>(string uri, T value)
	{
		OnStarted(HttpMethod.Post, uri);
		var response = await Client.PostAsJsonAsync(uri, value);
		bool success = false;

		try
		{
			response.EnsureSuccessStatusCode();
			success = true;
		}
		catch (Exception exc)
		{
			Logger.LogError(exc, "Error in {Method}", nameof(PostWithInputAsync));
			OnError(HttpMethod.Post, uri, exc);
		}
		finally
		{
			OnStopped(HttpMethod.Post, uri, success);
		}
	}

	protected async Task<TResult?> PostWithInputAndResultAsync<TResult>(string uri, TResult input)
	{
		OnStarted(HttpMethod.Post, uri);
		var response = await Client.PostAsJsonAsync(uri, input);
		bool success = false;

		try
		{
			response.EnsureSuccessStatusCode();
			success = true;
			return await response.Content.ReadFromJsonAsync<TResult>();
		}
		catch (Exception exc)
		{
			Logger.LogError(exc, "Error in {Method}", nameof(PostWithInputAndResultAsync));
			OnError(HttpMethod.Post, uri, exc);
		}
		finally
		{
			OnStopped(HttpMethod.Post, uri, success);
		}

		return default;
	}

	protected async Task DeleteAsync(string uri)
	{
		OnStarted(HttpMethod.Delete, uri);
		var response = await Client.DeleteAsync(uri);
		bool success = false;

		try
		{
			response.EnsureSuccessStatusCode();
			success = true;
		}
		catch (Exception exc)
		{
			Logger.LogError(exc, "Error in {Method}", nameof(DeleteAsync));
			OnError(HttpMethod.Delete, uri, exc);
		}
		finally
		{
			OnStopped(HttpMethod.Delete, uri, success);
		}
	}
}
